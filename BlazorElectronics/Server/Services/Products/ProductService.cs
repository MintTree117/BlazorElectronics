using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Server.Repositories.Products;
using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Server.Services.Specs;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Specs;
using BlazorElectronics.Shared.DtosInbound.Products;
using BlazorElectronics.Shared.DtosOutbound.Products;

namespace BlazorElectronics.Server.Services.Products;

public class ProductService : IProductService
{
    readonly IProductCache _cache;
    readonly IProductSearchRepository _productSearchRepository;
    readonly IProductDetailsRepository _productDetailsRepository;
    readonly IProductFeaturedRepository _productFeaturedRepository;

    readonly ICategoryService _categoryService;
    readonly ISpecService _specService;

    const int MAX_PRODUCT_LIST_ROWS = 100;

    public ProductService( 
        IProductCache cache, 
        IProductSearchRepository productSearchRepository, 
        IProductDetailsRepository productDetailsRepository, 
        ICategoryService categoryService, ISpecService specService, 
        IProductFeaturedRepository productFeaturedRepository )
    {
        _cache = cache;
        _productSearchRepository = productSearchRepository;
        _categoryService = categoryService;
        _specService = specService;
        _productFeaturedRepository = productFeaturedRepository;
        _productDetailsRepository = productDetailsRepository;
    }
    
    public async Task<ServiceResponse<string?>> GetProductSearchQueryString( ProductSearchRequest_DTO request )
    {
        ServiceResponse<int> categoryResult = await _categoryService.GetCategoryIdFromUrl( request.CategoryUrl! );
        
        if ( !categoryResult.IsSuccessful() )
            return new ServiceResponse<string?>( categoryResult.Message );

        int categoryId = categoryResult.Data;

        ProductSearchRequest productRequest = await GetValidatedSearchRequest( categoryId, request, _specService );
        
        string query = await _productSearchRepository.GetProductSearchQueryString( productRequest );
        return new ServiceResponse<string?>( query, true, "Test query" );
    }
    public async Task<ServiceResponse<Products_DTO?>> GetAllProducts()
    {
        IEnumerable<Product>? models = await _productSearchRepository.GetAll();
        
        if ( models == null )
            return new ServiceResponse<Products_DTO?>( "Failed to retrieve Products from repository!" );

        return new ServiceResponse<Products_DTO?> {
            Data = await MapProductsToDto( models ),
            Success = true,
            Message = "Successfully retrieved product Dto's from repository."
        };
    }
    public async Task<ServiceResponse<ProductsFeatured_DTO?>> GetFeaturedProducts()
    {
        ProductsFeatured_DTO? dto = await _cache.GetFeaturedProducts();

        if ( dto != null )
            return new ServiceResponse<ProductsFeatured_DTO?>( dto, true, "Success. Retrieved FeaturedProducts_DTO from cache." );

        IEnumerable<ProductFeatured>? models = await _productFeaturedRepository.GetAll();

        if ( models == null )
            return new ServiceResponse<ProductsFeatured_DTO?>( null, false, "Failed to retrieve FeaturedProducts_DTO from cache, and ProductDetails from repository!" );

        dto = await MapFeaturedProductsToDto( models );
        await _cache.CacheFeaturedProducts( dto );

        return new ServiceResponse<ProductsFeatured_DTO?>( dto, true, "Successfully retrieved FeaturedProducts_DTO from repository, mapped to DTO, and cached." );
    }
    public async Task<ServiceResponse<ProductSearchSuggestions_DTO?>> GetProductSearchSuggestions( string searchText )
    {
        IEnumerable<string>? result = await _productSearchRepository.GetSearchSuggestions( searchText );

        if ( result == null )
            return new ServiceResponse<ProductSearchSuggestions_DTO?>( null, false, "Failed to find any matching results!" );

        return new ServiceResponse<ProductSearchSuggestions_DTO?> {
            Data = await MapSearchSuggestionsToDto( result ),
            Success = true,
            Message = "Found matching results."
        };
    }
    public async Task<ServiceResponse<ProductSearchResults_DTO?>> GetProductSearch( ProductSearchRequest_DTO searchRequestDto )
    {
        int? categoryId = null;
        
        if ( searchRequestDto.CategoryUrl != null )
        {
            ServiceResponse<int> categoryIdResponse = await _categoryService.GetCategoryIdFromUrl( searchRequestDto.CategoryUrl );

            if ( !categoryIdResponse.IsSuccessful() )
                return new ServiceResponse<ProductSearchResults_DTO?>( categoryIdResponse.Message );
            
            categoryId = categoryIdResponse.Data;
        }
        
        ProductSearchRequest request = await GetValidatedSearchRequest( categoryId, searchRequestDto, _specService );
        ProductSearch? model = await _productSearchRepository.GetProductSearch( request );

        if ( model == null )
            return new ServiceResponse<ProductSearchResults_DTO?>( null, false, "Failed to retrieve ProductSearch from repository!" );

        return new ServiceResponse<ProductSearchResults_DTO?> {
            Data = await MapProductSearchToDto( model ),
            Success = true,
            Message = "Successfully retrieved ProductSearch from repository."
        };
    }
    public async Task<ServiceResponse<ProductDetails_DTO?>> GetProductDetails( int productId )
    {
        ProductDetails_DTO? dto = await _cache.GetProductDetails( productId );

        if ( dto != null )
            return new ServiceResponse<ProductDetails_DTO?>( dto, true, "Success. Retrieved ProductDetails_DTO from cache." );

        ProductDetails? model = await _productDetailsRepository.GetById( productId );

        if ( model == null )
            return new ServiceResponse<ProductDetails_DTO?>( null, false, "Failed to retrieve ProductDetails_DTO from cache, and ProductDetails from repository!" );

        dto = await MapProductDetailsToDto( model );
        await _cache.CacheProductDetails( dto );

        return new ServiceResponse<ProductDetails_DTO?>( dto, true, "Successfully retrieved ProductDetails from repository, mapped to DTO, and cached." );
    }

    static async Task<ProductSearchRequest> GetValidatedSearchRequest( int? categoryId, ProductSearchRequest_DTO searchRequestDto, ISpecService specService )
    {
        var validatedFilters = new ProductSearchRequest 
        {
            Page = Math.Max( searchRequestDto.Page, 0 ),
            Rows = Math.Clamp( searchRequestDto.NumberOfResults, 0, MAX_PRODUCT_LIST_ROWS ),
            MinPrice = searchRequestDto.MinPrice == null ? null : Math.Max( searchRequestDto.MinPrice.Value, 0 ),
            MaxPrice = searchRequestDto.MaxPrice == null ? null : Math.Max( searchRequestDto.MaxPrice.Value, 0 ),
            MinRating = searchRequestDto.MinRating == null ? null : Math.Max( searchRequestDto.MinRating.Value, 0 ),
            MaxRating = searchRequestDto.MaxRating == null ? null : Math.Max( searchRequestDto.MaxRating.Value, 0 ),
            SearchText = string.IsNullOrEmpty( searchRequestDto.SearchText ) ? null : searchRequestDto.SearchText,
            CategoryId = categoryId
        };
        
        if ( searchRequestDto.SpecFilters == null || categoryId == null )
            return validatedFilters;
        
        ServiceResponse<SpecFilters_DTO> specsResponse = await specService.GetSpecFilters( categoryId.Value );
        if ( !specsResponse.Success || specsResponse.Data == null )
            return validatedFilters;
        
        SpecFilters_DTO? filters = specsResponse.Data;
        validatedFilters.LookupSpecFilters = new List<ProductSpecFilter>();
        
        await Task.Run( () =>
        {
            foreach ( ProductSpecFilter_DTO specFilterDTO in searchRequestDto.SpecFilters )
            {
                if ( string.IsNullOrEmpty( specFilterDTO.SpecName ) )
                    continue;
                if ( specFilterDTO.SpecValue == null )
                    continue;
                if ( !filters.IndicesByName.TryGetValue( specFilterDTO.SpecName, out int specIndex ) )
                    continue;
                if ( specIndex < 0 || specIndex >= filters.Filters.Count )
                    continue;
                
                validatedFilters.LookupSpecFilters.Add( new ProductSpecFilter {
                    SpecId = filters.Filters[ specIndex ].Id,
                    SpecName = specFilterDTO.SpecName,
                    SpecValue = specFilterDTO.SpecValue,
                } );
            }
        } );

        return validatedFilters;
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
    static async Task<ProductsFeatured_DTO> MapFeaturedProductsToDto( IEnumerable<ProductFeatured> models )
    {
        var dtos = new List<ProductFeatured_DTO>();

        await Task.Run( () =>
        {
            foreach ( ProductFeatured f in models )
            {
                dtos.Add( new ProductFeatured_DTO {
                    ProductId = f.ProductId,
                    ImageUrl = f.FeatureImageUrl
                } );
            }
        } );

        return new ProductsFeatured_DTO {
            FeaturedProducts = dtos
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
            dto.Rows = model.QueryRows;
            dto.Offset = model.QueryOffset;
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
}