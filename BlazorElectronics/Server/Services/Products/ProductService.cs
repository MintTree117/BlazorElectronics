using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Server.Repositories.Products;
using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Server.Services.Specs;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Products;
using BlazorElectronics.Shared.DataTransferObjects.Specs;

namespace BlazorElectronics.Server.Services.Products;

public class ProductService : IProductService
{
    readonly IProductCache _cache;
    readonly IProductRepository _productRepository;
    readonly IProductDetailsRepository _productDetailsRepository;

    readonly ICategoryService _categoryService;
    readonly ISpecService _specService;

    const int MAX_PRODUCT_LIST_ROWS = 100;

    public ProductService( IProductCache cache, IProductRepository productRepository, IProductDetailsRepository productDetailsRepository, ICategoryService categoryService, ISpecService specService )
    {
        _cache = cache;
        _productRepository = productRepository;
        _categoryService = categoryService;
        _specService = specService;
        _productDetailsRepository = productDetailsRepository;
    }
    
    public async Task<ServiceResponse<string?>> TestGetQueryString( string categoryUrl, ProductSearchFilters_DTO filters )
    {
        ServiceResponse<int> categoryResult = await _categoryService.GetCategoryIdFromUrl( categoryUrl );
        
        if ( !categoryResult.IsSuccessful() )
            return new ServiceResponse<string?>( categoryResult.Message );

        int categoryId = categoryResult.Data;
        
        string query = await _productRepository.TEST_GET_QUERY_STRING( categoryId, new ValidatedSearchFilters() );
        return new ServiceResponse<string?>( query, true, "Test query" );
    }
    public async Task<ServiceResponse<Products_DTO?>> GetProducts()
    {
        IEnumerable<Product>? models = await _productRepository.GetAll();
        
        if ( models == null )
            return new ServiceResponse<Products_DTO?>( "Failed to retrieve Products from repository!" );
        
        Products_DTO productsDto = await MapProductsToDto( models );
        
        return new ServiceResponse<Products_DTO?>( productsDto, true, "Successfully retrieved product Dto's from repository." );
    }
    public async Task<ServiceResponse<ProductSearchSuggestions_DTO?>> GetProductSearchSuggestions( string searchText )
    {
        IEnumerable<string>? result = await _productRepository.GetSearchSuggestions( searchText );

        if ( result == null )
            return new ServiceResponse<ProductSearchSuggestions_DTO?>( null, false, "Failed to find any matching results!" );

        ProductSearchSuggestions_DTO dto = await MapSearchSuggestionsToDto( result );
        return new ServiceResponse<ProductSearchSuggestions_DTO?>( dto, true, "Found matching results." );
    }
    public async Task<ServiceResponse<ProductSearch_DTO?>> SearchProducts( ProductSearchFilters_DTO? searchFiltersDTO )
    {
        if ( searchFiltersDTO == null )
            return new ServiceResponse<ProductSearch_DTO?>( null, false, "No filters " );

        int? categoryId = null;
        
        if ( searchFiltersDTO.CategoryUrl != null )
        {
            ServiceResponse<int> categoryResult = await _categoryService.GetCategoryIdFromUrl( searchFiltersDTO.CategoryUrl );

            if ( !categoryResult.IsSuccessful() )
                return new ServiceResponse<ProductSearch_DTO?>( categoryResult.Message );

            categoryId = categoryResult.Data;
        }
        
        ValidatedSearchFilters filters = await GetValidatedSearchFilters( categoryId, searchFiltersDTO, _specService );

        ProductSearch? model = await _productRepository.SearchProducts( filters );

        if ( model == null )
            return new ServiceResponse<ProductSearch_DTO?>( null, false, "Failed to retrieve ProductSearch from repository!" );

        ProductSearch_DTO? dto = await MapProductSearchToDto( model );
        return new ServiceResponse<ProductSearch_DTO?>( dto, true, "Successfully retrieved ProductSearch from repository." );
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

    static async Task<ValidatedSearchFilters> GetValidatedSearchFilters( int? categoryId, ProductSearchFilters_DTO searchFiltersDTO, ISpecService specService )
    {
        var validatedFilters = new ValidatedSearchFilters();
        
        // BASE
        validatedFilters.Page = Math.Max( searchFiltersDTO.Page, 0 );
        validatedFilters.Rows = Math.Max( searchFiltersDTO.Rows, 0 );
        validatedFilters.Rows = Math.Min( searchFiltersDTO.Rows, MAX_PRODUCT_LIST_ROWS );
        validatedFilters.MinPrice = searchFiltersDTO.MinPrice < 0 ? null : Math.Max( searchFiltersDTO.MinPrice, 0 );
        validatedFilters.MaxPrice = searchFiltersDTO.MaxPrice < 0 ? null : Math.Max( searchFiltersDTO.MaxPrice, 0 );
        validatedFilters.MinRating = searchFiltersDTO.MinRating < 0 ? null : Math.Max( searchFiltersDTO.MinRating, 0 );
        validatedFilters.MaxRating = searchFiltersDTO.MaxRating < 0 ? null : Math.Max( searchFiltersDTO.MaxRating, 0 );
        validatedFilters.SearchText = string.IsNullOrEmpty( searchFiltersDTO.SearchText ) ? null : searchFiltersDTO.SearchText;
        
        // NO SPECS
        if ( searchFiltersDTO.SpecFilters == null || categoryId == null )
            return validatedFilters;
        
        // HAS SPECS
        ServiceResponse<SpecFilters_DTO> specsTask = await specService.GetSpecFilters( categoryId.Value );
        if ( !specsTask.Success || specsTask.Data == null )
            return validatedFilters;

        SpecFilters_DTO? filters = specsTask.Data;
        
        await Task.Run( () =>
        {
            foreach ( ProductSpecFilter_DTO specFilterDTO in searchFiltersDTO.SpecFilters )
            {
                if ( string.IsNullOrEmpty( specFilterDTO.SpecName ) )
                    continue;
                if ( specFilterDTO.SpecValue == null )
                    continue;
                if ( !filters.IndicesByName.TryGetValue( specFilterDTO.SpecName, out int specIndex ) )
                    continue;
                if ( specIndex < 0 || specIndex >= filters.Filters.Count )
                    continue;

                SpecFilter_DTO filter = filters.Filters[ specIndex ];

                validatedFilters.LookupSpecFilters.Add( new ProductSpecFilter {
                    SpecId = filter.Id,
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
    static async Task<ProductSearch_DTO> MapProductSearchToDto( ProductSearch model )
    {
        var dto = new ProductSearch_DTO();

        await Task.Run( () =>
        {
            foreach ( Product p in model.Products! )
                dto.Products.Add( MapProductToDto( p ) );
            dto.NumResults = model.Count;
        } );

        return dto;
    }
    static async Task<ProductDetails_DTO> MapProductDetailsToDto( ProductDetails productDetails )
    {
        var dto = new ProductDetails_DTO();

        await Task.Run( () =>
        {
            dto.ProductId = productDetails.Product.ProductId;
            dto.ProductName = productDetails.Product.ProductName;

            if ( productDetails.ProductDescription != null )
            {
                dto.ProductDescription = productDetails.ProductDescription.Description ?? "No description!";
            }

            foreach ( ProductVariant variant in productDetails.Product.ProductVariants )
            {
                dto.ProductVariants.Add( new ProductVariant_DTO {
                    VariantId = variant.VariantId,
                    VariantName = variant.VariantName ?? "No name!",
                    Price = variant.VariantPriceMain,
                    SalePrice = variant.VariantPriceSale,
                } );
            }

            foreach ( ProductImage image in productDetails.ProductImages )
            {
                dto.ProductImages.Add( new ProductImage_DTO {
                    Url = image.ImageUrl,
                    Variant = image.ProductVariantId
                } );
            }

            foreach ( ProductReview review in productDetails.ProductReviews )
            {
                dto.ProductReviews.Add( new ProductReview_DTO {
                    Rating = review.ReviewScore,
                    Review = review.ReviewBody,
                    User = review.UserId
                } );
            }
        } );

        return dto;
    }
    static Product_DTO MapProductToDto( Product p )
    {
        var productDto = new Product_DTO {
            Id = p.ProductId,
            Title = p.ProductName,
            Thumbnail = p.ProductThumbnail,
            Rating = p.ProductRating
        };
        foreach ( ProductVariant v in p.ProductVariants )
        {
            productDto.Variants.Add( new ProductVariant_DTO {
                VariantId = v.VariantId,
                VariantName = v.VariantName ?? "No name!",
                Price = v.VariantPriceMain,
                SalePrice = v.VariantPriceSale
            } );
        }

        return productDto;
    }
}