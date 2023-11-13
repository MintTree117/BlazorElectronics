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

    public Task<ApiReply<string?>> GetProductSearchQueryString( ProductSearchRequest request )
    {
        throw new NotImplementedException();
    }
    public async Task<ApiReply<ProductSearchSuggestions_DTO?>> GetProductSuggestions( ProductSuggestionRequest request )
    {
        Task<IEnumerable<string>?> repoFunction = _productSearchRepository.GetSearchSuggestions( request.SearchText!, request.CategoryIdMap!.CategoryTier, request.CategoryIdMap.CategoryId );
        ApiReply<IEnumerable<string>?> repoReply = await ExecuteIoCall( async () => await repoFunction );

        if ( !repoReply.Success )
            return new ApiReply<ProductSearchSuggestions_DTO?>( repoReply.Message );

        return new ApiReply<ProductSearchSuggestions_DTO?> {
            Data = await MapSearchSuggestionsToResponse( repoReply.Data! ),
            Success = true,
            Message = "Found matching results."
        };
    }
    public async Task<ApiReply<ProductSearchResponse?>> GetProductSearch( CategoryIdMap? categoryIdMap, ProductSearchRequest? request, CachedSpecData specData )
    {
        ApiReply<ProductSearchRequest> validateReply = await GetValidatedProductSearchRequest( request );

        if ( !validateReply.Success || validateReply.Data is null )
            return new ApiReply<ProductSearchResponse?>( validateReply.Message );

        IEnumerable<ProductSearchModel>? models;

        try
        {
            models = await _productSearchRepository.GetProductSearch( categoryIdMap, validateReply.Data, specData );

            if ( models is null )
                return new ApiReply<ProductSearchResponse?>( NO_DATA_FOUND_MESSAGE );
        }
        catch ( ServiceException e )
        {
            _logger.LogError( e.Message, e );
            return new ApiReply<ProductSearchResponse?>( INTERNAL_SERVER_ERROR_MESSAGE );
        }

        return new ApiReply<ProductSearchResponse?>( await MapProductSearchToResponse( models ) );
    }
    public async Task<ApiReply<ProductDetails_DTO?>> GetProductDetails( int productId )
    {
        Task<ProductDetails_DTO?> cacheFetchFunction = _cache.GetProductDetails( productId );
        var cacheFetchResult = ExecuteIoCall( async () => await cacheFetchFunction );
        
        
        ProductDetails_DTO? dto;

        try
        {
            dto = await _cache.GetProductDetails( productId );
            if ( dto != null )
                return new ApiReply<ProductDetails_DTO?>( dto, true, "Successfully got Product Details from cache." );
        }
        catch ( ServiceException e )
        {
            return new ApiReply<ProductDetails_DTO?>( e.Message );
        }

        ProductDetails? model;

        try
        {
            model = await _productDetailsRepository.GetProductDetailsById( productId );
        }
        catch ( ServiceException e )
        {
            return new ApiReply<ProductDetails_DTO?>( e.Message );
        }

        dto = await MapProductDetailsToResponse( model! );

        try
        {
            await _cache.CacheProductDetails( dto );
        }
        catch ( ServiceException e )
        {
            return new ApiReply<ProductDetails_DTO?>( dto, true, $"Successfully retrieved ProductDetails from repository, but failed to cache with message: {e.Message}" );
        } 

        return new ApiReply<ProductDetails_DTO?>( dto, true, "Successfully retrieved ProductDetails from repository, and cached." );
    }

    static async Task<ApiReply<ProductSearchRequest>> GetValidatedProductSearchRequest( ProductSearchRequest? request )
    {
        if ( request == null )
            return new ApiReply<ProductSearchRequest>( new ProductSearchRequest(), true, "Validated Search Request." );

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
    static async Task<ProductSearchSuggestions_DTO> MapSearchSuggestionsToResponse( IEnumerable<string> suggestions )
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
    static async Task<ProductSearchResponse> MapProductSearchToResponse( IEnumerable<ProductSearchModel> models )
    {
        var dto = new ProductSearchResponse();

        await Task.Run( () =>
        {
            foreach ( ProductSearchModel p in models )
            {
                dto.Products.Add( new ProductResponse
                {
                    Id = p.ProductId,
                    Title = p.ProductTitle,
                    Thumbnail = p.ProductThumbnail,
                    Rating = p.ProductRating
                } );
            }
        } );

        return dto;
    }
    static async Task<ProductDetails_DTO> MapProductDetailsToResponse( ProductDetails productDetails )
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
}