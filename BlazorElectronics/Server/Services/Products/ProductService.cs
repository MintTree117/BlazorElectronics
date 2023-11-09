using BlazorElectronics.Server.Caches.Products;
using BlazorElectronics.Server.Dtos.Specs;
using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Server.Repositories.Products;
using BlazorElectronics.Server.Services.Specs;
using BlazorElectronics.Shared.DtosOutbound.Products;
using BlazorElectronics.Shared.Inbound.Products;
using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Server.Services.Products;

public class ProductService : ApiService, IProductService
{
    readonly IProductCache _cache;
    readonly IProductSearchRepository _productSearchRepository;
    readonly IProductDetailsRepository _productDetailsRepository;
    readonly ISpecLookupService _specLookupService;

    const int MAX_PRODUCT_LIST_ROWS = 100;
    const int MAX_SEARCH_TEXT_LENGTH = 64;
    const int MAX_FILTER_ID_LENGTH = 8;

    public ProductService(
        ILogger logger,
        IProductCache cache, 
        IProductSearchRepository productSearchRepository, 
        IProductDetailsRepository productDetailsRepository,
        ISpecLookupService specLookupService )
        : base( logger )
    {
        _cache = cache;
        _productSearchRepository = productSearchRepository;
        _specLookupService = specLookupService;
        _productDetailsRepository = productDetailsRepository;
    }

    public Task<Reply<string?>> GetProductSearchQueryString( ProductSearchRequest request )
    {
        throw new NotImplementedException();
    }
    public async Task<Reply<Products_DTO?>> GetAllProducts()
    {
        /*Task<IEnumerable<Product>?> repoFunction = _productSearchRepository.GetAllProducts();
        Reply<IEnumerable<Product>?> repositoryReply = await ExecuteIoCall( async () => await repoFunction );

        if ( !repositoryReply.Success )
            return new Reply<Products_DTO?>( repositoryReply.Message );
        
        return new Reply<Products_DTO?> {
            Data = await MapProductsToDto( repositoryReply.Data! ),
            Success = true,
            Message = "Successfully retrieved product Dto's from repository."
        };*/

        return null;
    }
    public async Task<Reply<ProductSearchSuggestions_DTO?>> GetProductSuggestions( ProductSuggestionRequest request )
    {
        Task<IEnumerable<string>?> repoFunction = _productSearchRepository.GetSearchSuggestions( request.SearchText!, request.CategoryIdMap!.CategoryTier, request.CategoryIdMap.CategoryId );
        Reply<IEnumerable<string>?> repoReply = await ExecuteIoCall( async () => await repoFunction );

        if ( !repoReply.Success )
            return new Reply<ProductSearchSuggestions_DTO?>( repoReply.Message );

        return new Reply<ProductSearchSuggestions_DTO?> {
            Data = await MapSearchSuggestionsToDto( repoReply.Data! ),
            Success = true,
            Message = "Found matching results."
        };
    }
    public async Task<Reply<ProductSearchResults_DTO?>> GetProductSearch( CategoryIdMap? categoryIdMap, ProductSearchRequest? request, CachedSpecData specMeta )
    {
        Reply<ProductSearchRequest> validateReply = await GetValidatedProductSearchRequest( request );

        if ( !validateReply.Success )
            return new Reply<ProductSearchResults_DTO?>( validateReply.Message );

        Task<ProductSearch?> repoFunction = _productSearchRepository.GetProductSearch( categoryIdMap, validateReply.Data!, specMeta );
        Reply<ProductSearch?> repoReply = await ExecuteIoCall( async () => await repoFunction );

        if ( !repoReply.Success )
            return new Reply<ProductSearchResults_DTO?>( repoReply.Message );
        
        return new Reply<ProductSearchResults_DTO?> {
            Data = await MapProductSearchToDto( repoReply.Data! ),
            Success = true,
            Message = "Successfully retrieved ProductSearch from repository."
        };
    }
    public async Task<Reply<ProductDetails_DTO?>> GetProductDetails( int productId )
    {
        Task<ProductDetails_DTO?> cacheFetchFunction = _cache.GetProductDetails( productId );
        var cacheFetchResult = ExecuteIoCall( async () => await cacheFetchFunction );
        
        
        ProductDetails_DTO? dto;

        try
        {
            dto = await _cache.GetProductDetails( productId );
            if ( dto != null )
                return new Reply<ProductDetails_DTO?>( dto, true, "Successfully got Product Details from cache." );
        }
        catch ( ServiceException e )
        {
            return new Reply<ProductDetails_DTO?>( e.Message );
        }

        ProductDetails? model;

        try
        {
            model = await _productDetailsRepository.GetProductDetailsById( productId );
        }
        catch ( ServiceException e )
        {
            return new Reply<ProductDetails_DTO?>( e.Message );
        }

        dto = await MapProductDetailsToDto( model! );

        try
        {
            await _cache.CacheProductDetails( dto );
        }
        catch ( ServiceException e )
        {
            return new Reply<ProductDetails_DTO?>( dto, true, $"Successfully retrieved ProductDetails from repository, but failed to cache with message: {e.Message}" );
        } 

        return new Reply<ProductDetails_DTO?>( dto, true, "Successfully retrieved ProductDetails from repository, and cached." );
    }

    static async Task<Reply<ProductSearchRequest>> GetValidatedProductSearchRequest( ProductSearchRequest? request )
    {
        if ( request == null )
            return new Reply<ProductSearchRequest>( new ProductSearchRequest(), true, "Validated Search Request." );

        request.Page = Math.Max( request.Page, 0 );
        request.Rows = Math.Clamp( request.Rows, 0, MAX_PRODUCT_LIST_ROWS );
        request.MinPrice = request.MinPrice == null ? null : Math.Max( request.MinPrice.Value, 0 );
        request.MaxPrice = request.MaxPrice == null ? null : Math.Max( request.MaxPrice.Value, 0 );
        request.MinRating = request.MinRating == null ? null : Math.Max( request.MinRating.Value, 0 );
        request.MaxRating = request.MaxRating == null ? null : Math.Max( request.MaxRating.Value, 0 );
        request.SearchText = request.SearchText;

        if ( request.SearchText?.Length > MAX_SEARCH_TEXT_LENGTH )
            request.SearchText.Remove( MAX_SEARCH_TEXT_LENGTH - 1 );
        
        return null;
    }
    static async Task<Products_DTO> MapProductsToDto( IEnumerable<Product> models )
    {
        var dto = new Products_DTO();

        await Task.Run( () =>
        {
            foreach ( Product p in models )
                dto.Products.Add( MapProductToDto( p ) );
        } );
        
        return dto;
    }
    static async Task<ProductSearchSuggestions_DTO> MapSearchSuggestionsToDto( IEnumerable<string> suggestions )
    {
        var suggestionList = new List<string>();

        await Task.Run( () =>
        {
            suggestionList = suggestions.ToList();
        } );

        return new ProductSearchSuggestions_DTO {
            Suggestions = suggestionList
        };
    }
    static async Task<ProductSearchResults_DTO> MapProductSearchToDto( ProductSearch model )
    {
        var dto = new ProductSearchResults_DTO();

        await Task.Run( () =>
        {
            foreach ( Product p in model.Products! )
                dto.Products.Add( MapProductToDto( p ) );
            
            dto.TotalMatches = model.TotalSearchCount;
            dto.TotalPages = model.TotalSearchCount / model.QueryRows;
            dto.ItemsPerPage = model.QueryRows;
            dto.CurrentPage = model.QueryOffset + 1;
        } );

        return dto;
    }
    static async Task<ProductDetails_DTO> MapProductDetailsToDto( ProductDetails productDetails )
    {
        var dto = new ProductDetails_DTO();

        await Task.Run( () =>
        {
            if ( productDetails.Product == null )
                return;
            
            dto.Id = productDetails.Product.ProductId;
            dto.Title = productDetails.Product.ProductTitle;

            if ( productDetails.ProductDescription != null )
            {
                dto.Description = productDetails.ProductDescription.DescriptionBody ?? "No description!";
            }

            foreach ( ProductVariant variant in productDetails.Product.ProductVariants )
            {
                dto.Variants.Add( new ProductVariant_DTO {
                    Id = variant.VariantId,
                    Name = variant.VariantName ?? "No Variant Name!",
                    Price = variant.VariantPriceMain,
                    SalePrice = variant.VariantPriceSale,
                } );
            }

            foreach ( ProductImage image in productDetails.ProductImages )
            {
                dto.Images.Add( new ProductImage_DTO {
                    Url = image.ImageUrl,
                    VariantId = image.ProductVariantId
                } );
            }

            foreach ( ProductReview review in productDetails.ProductReviews )
            {
                dto.Reviews.Add( new ProductReview_DTO {
                    Rating = review.ReviewScore,
                    Review = review.ReviewBody,
                    UserId = review.UserId
                } );
            }
        } );

        return dto;
    }
    static Product_DTO MapProductToDto( Product p )
    {
        var productDto = new Product_DTO {
            Id = p.ProductId,
            Title = p.ProductTitle,
            Thumbnail = p.ProductThumbnail,
            Rating = p.ProductRating
        };
        foreach ( ProductVariant v in p.ProductVariants )
        {
            productDto.Variants.Add( new ProductVariant_DTO {
                Id = v.VariantId,
                Name = v.VariantName ?? "No name!",
                Price = v.VariantPriceMain,
                SalePrice = v.VariantPriceSale
            } );
        }

        return productDto;
    }

    static void ValidateProductSearchRequestBooks()
    {
        
    }
    static void ValidateProductSearchRequestSoftware() { }
    static void ValidateProductSearchRequestGames() { }
    static void ValidateProductSearchRequestMoviesTv() { }
    static void ValidateProductSearchRequestCourses() { }
}