using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Server.Models.Specs;
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
    
    public async Task<DtoResponse<string?>> TestGetQueryString( string categoryUrl, ProductSearchFilters_DTO filters )
    {
        DtoResponse<int> categoryResult = await _categoryService.GetCategoryIdFromUrl( categoryUrl );
        if ( !categoryResult.IsSuccessful() )
            return new DtoResponse<string?>( categoryResult.Message );

        int categoryId = categoryResult.Data;
        
        string query = await _productRepository.TEST_GET_QUERY_STRING( categoryId, new ValidatedSearchFilters() );
        return new DtoResponse<string?>( query, true, "Test query" );
    }
    public async Task<DtoResponse<Products_DTO?>> GetProducts()
    {
        IEnumerable<Product> models = await _productRepository.GetAll();

        if ( models == null )
            return new DtoResponse<Products_DTO?>( "Failed to retrieve Products from repository!" );

        Products_DTO productsDto = await MapProductsToDto( models );
        
        return new DtoResponse<Products_DTO?>( productsDto, true, "Successfully retrieved product Dto's from repository." );
    }
    public async Task<DtoResponse<ProductSearch_DTO?>> SearchProducts( string categoryUrl, ProductSearchFilters_DTO? searchFiltersDTO )
    {
        DtoResponse<int> categoryResult = await _categoryService.GetCategoryIdFromUrl( categoryUrl );
        if ( !categoryResult.IsSuccessful() )
            return new DtoResponse<ProductSearch_DTO?>( categoryResult.Message );

        int categoryId = categoryResult.Data;
        var filters = new ValidatedSearchFilters();

        if ( searchFiltersDTO != null )
            filters = await GetValidatedSearchFilters( categoryId, searchFiltersDTO );

        (IEnumerable<Product>?, int)? models = await _productRepository.SearchProducts( categoryId, filters );

        if ( models?.Item1 == null || models.Value.Item2 < 0 )
            return new DtoResponse<ProductSearch_DTO?>( null, false, "Failed to retrieve ProductSearch from repository!" );

        ProductSearch_DTO? dto = await MapProductSearchToDto( models );

        return new DtoResponse<ProductSearch_DTO?>( dto, true, "Successfully retrieved ProductSearch from repository." );
    }
    public async Task<DtoResponse<ProductDetails_DTO?>> GetProductDetails( int productId )
    {
        ProductDetails_DTO? dto = await _cache.GetProductDetails( productId );

        if ( dto != null )
            return new DtoResponse<ProductDetails_DTO?>( dto, true, "Success. Retrieved ProductDetails_DTO from cache." );

        ProductDetails? model = await _productDetailsRepository.GetById( productId );

        if ( model == null )
            return new DtoResponse<ProductDetails_DTO?>( null, false, "Failed to retrieve ProductDetails_DTO from cache, and ProductDetails from repository!" );

        dto = await MapProductDetailsToDto( model );
        await _cache.CacheProductDetails( dto );

        return new DtoResponse<ProductDetails_DTO?>( dto, true, "Successfully retrieved ProductDetails from repository, mapped to DTO, and cached." );
    }
    
    async Task<ValidatedSearchFilters> GetValidatedSearchFilters( int categoryId, ProductSearchFilters_DTO searchFiltersDTO )
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
        if ( searchFiltersDTO.SpecFilters == null )
            return validatedFilters;
        // HAS SPECS
        Task<DtoResponse<CachedSpecDescrs?>> specsTask = null; //_specService.GetSpecsDTO();
        Task<DtoResponse<SpecValues_DTO?>> lookupsTask = null; //_specService.GetSpecLookupsDTO();

        await Task.WhenAll( specsTask, lookupsTask );

        if ( specsTask.Result.Data == null || !specsTask.Result.Success )
            return validatedFilters;
        if ( lookupsTask.Result.Data == null || !lookupsTask.Result.Success )
            return validatedFilters;

        CachedSpecDescrs? specs = specsTask.Result.Data;
        SpecValues_DTO? lookups = lookupsTask.Result.Data;

        await Task.Run( () =>
        {
            foreach ( ProductSpecFilter_DTO specFilterDTO in searchFiltersDTO.SpecFilters )
            {
                if ( string.IsNullOrEmpty( specFilterDTO.SpecName ) )
                    continue;
                if ( specFilterDTO.SpecValue == null )
                    continue;
                if ( !specs.IdsByName.TryGetValue( specFilterDTO.SpecName, out int specId ) )
                    continue;
                if ( !specs.SpecsById.TryGetValue( specId, out Spec_DTO? spec ) || spec == null )
                    continue;
                //if ( validatedFilters.Category != null && !spec.SpecCategoryIds.Contains( validatedFilters.Category.Value ) )
                    //continue;
                //if ( !Enum.IsDefined( typeof( SpecFilterType ), specFilterDTO.FilterType ) )
                    //continue;
                /*
                var specType = ( SpecType ) specs.SpecsById[ specId ].SpecType;
                
                if (specType == SpecType.Lookup && !ValidateSpecLookup( specId,  ) )
                
                if ( specFilterDTO.SpecType == ( int ) SpecType.Lookup && !ValidateSpecLookup( specId, specFilterDTO, lookups ) )
                    continue;
                if ( !ValidateSpecLookup( specId, specFilterDTO, lookups ) )
                    continue;
                validatedFilters.LookupSpecFilters.Add( new ProductSpecFilter {
                    SpecName = specFilterDTO.SpecName,
                    SpecValue = specFilterDTO.SpecValue,
                    DataType = ( SpecDataType ) specFilterDTO.DataType,
                    FilterType = ( SpecFilterType ) specFilterDTO.FilterType,
                    SpecType = ( SpecType ) specFilterDTO.SpecType
                } );*/
            }
        } );

        return validatedFilters;
    }
    static async Task<Products_DTO> MapProductsToDto( IEnumerable<Product>? models )
    {
        var dto = new Products_DTO();

        await Task.Run( () =>
        {
            foreach ( Product p in models )
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
                        VariantName = v.VariantName,
                        Price = v.VariantPriceMain,
                        SalePrice = v.VariantPriceSale
                    } );
                }
                dto.Products.Add( productDto );
            }
        } );
        
        return dto;
    }
    static async Task<ProductSearch_DTO> MapProductSearchToDto( (IEnumerable<Product>?, int)? model )
    {
        var dto = new ProductSearch_DTO();

        return dto;
    }
    static async Task<ProductDetails_DTO> MapProductDetailsToDto( ProductDetails productDetails )
    {
        var DTO = new ProductDetails_DTO {
            ProductId = productDetails.Product.ProductId,
            ProductName = productDetails.Product.ProductName,
            ProductDescription = productDetails.ProductDescription.DescriptionBody
        };

        foreach ( ProductVariant variant in productDetails.Product.ProductVariants ) {
            DTO.ProductVariants.Add( new ProductVariant_DTO {
                VariantId = variant.VariantId,
                VariantName = variant.VariantName,
                Price = variant.VariantPriceMain,
                SalePrice = variant.VariantPriceSale,
            } );
        }

        foreach ( ProductImage image in productDetails.ProductImages ) {
            DTO.ProductImages.Add( new ProductImage_DTO {
                Url = image.ImageUrl,
                Variant = image.ProductVariantId
            } );
        }

        foreach ( ProductReview review in productDetails.ProductReviews ) {
            DTO.ProductReviews.Add( new ProductReview_DTO {
                Rating = review.ReviewScore,
                Review = review.ReviewBody,
                User = review.UserId
            } );
        }

        return DTO;
    }
    
    static bool ValidateSpecLookup( int specId, ProductSpecFilter_DTO specFilterDTO, Dictionary<int, Dictionary<int, object>> specValueLookup )
    {
        return false;
        /*if ( specFilterDTO.SpecType != ( int ) SpecType.Lookup )
            return true;
        
        if (specValueLookup.TryGetValue( specFilterDTO. ))
        
        return specValueLookup.StaticValuesBySpecId.TryGetValue( specId, out Dictionary<int, object>? values )
               && values.Any( o => specFilterDTO.SpecValue == o );*/

        return false;
    }
    static bool ValidateSpecFilterValue( Spec_DTO specDto, int valueId, object value, SpecValues_DTO lookups )
    {
        /*if ( specDto.SpecType == ( int ) SpecType.Lookup )
        {
            if ( !lookups.LookupValuesBySpecId.TryGetValue( specDto.Id, out Dictionary<int, object>? values ) )
                return false;
            if ( !values.TryGetValue( valueId, out object? staticValue ) )
                return false;
            return value == staticValue;
        }
        else
        {
            if ( !lookups.DynamicLimitsBySpecId.TryGetValue( specDto.Id, out Dictionary<int, SpecDynamicValueLimits_DTO>? limits ) )
                return false;
            if ( !limits.TryGetValue( valueId, out SpecDynamicValueLimits_DTO? limit ) )
                return false;
            return false;
        }*/
        return false;
    }
}