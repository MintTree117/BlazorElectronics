using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Server.Models.Specs;
using BlazorElectronics.Server.Repositories.Products;
using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Server.Services.Specs;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Categories;
using BlazorElectronics.Shared.DataTransferObjects.Products;
using BlazorElectronics.Shared.DataTransferObjects.Specs;

namespace BlazorElectronics.Server.Services.Products;

public class ProductService : IProductService
{
    readonly IProductCache _cache;
    readonly IProductRepository _repository;

    readonly ICategoryService _categoryService;
    readonly ISpecService _specService;

    const int MAX_PRODUCT_LIST_ROWS = 100;

    public ProductService( IProductCache cache, IProductRepository repository, ICategoryService categoryService, ISpecService specService )
    {
        _cache = cache;
        _repository = repository;
        _categoryService = categoryService;
        _specService = specService;
    }
    
    public async Task<DtoResponse<string?>> TestGetQueryString( ProductSearchFilters_DTO filters )
    {
        string query = await _repository.TEST_GET_QUERY_STRING( new ValidatedSearchFilters() );
        return new DtoResponse<string?>( query, true, "Test query" );
    }
    public async Task<DtoResponse<Products_DTO?>> GetProducts( ProductSearchFilters_DTO? searchFilters = null )
    {
        DtoResponse<(IEnumerable<Product>?, int)?> getResult = searchFilters == null
            ? await GetAllProducts()
            : await SearchProducts( searchFilters );

        if ( !getResult.Success || getResult.Data?.Item1 == null )
            return new DtoResponse<Products_DTO?>( getResult.Message );

        Products_DTO productsDto = await MapProductsToDto( getResult.Data.Value.Item1, getResult.Data.Value.Item2 );
        
        return new DtoResponse<Products_DTO?>( productsDto, true, "Successfully retrieved product Dto's from repository." );
    }
    public async Task<DtoResponse<ProductDetails_DTO?>> GetProductDetails( int productId )
    {
        ProductDetails_DTO? dto = await _cache.GetProductDetails( productId );

        if ( dto != null )
            return new DtoResponse<ProductDetails_DTO?>( dto, true, "Success. Retrieved ProductDetails_DTO from cache." );

        ProductDetails? model = await _repository.GetProductDetails( productId );

        if ( model == null )
            return new DtoResponse<ProductDetails_DTO?>( null, false, "Failed to retrieve ProductDetails_DTO from cache, and ProductDetails from repository!" );

        dto = await MapProductDetailsToDto( model );
        await _cache.CacheProductDetails( dto );

        return new DtoResponse<ProductDetails_DTO?>( dto, true, "Successfully retrieved ProductDetails from repository, mapped to DTO, and cached." );
    }

    async Task<DtoResponse<(IEnumerable<Product>?, int)?>> GetAllProducts()
    {
        (IEnumerable<Product>?, int)? result = await _repository.GetAllProducts();
        
        return result is { Item1: not null, Item2: > 0 }
            ? new DtoResponse<(IEnumerable<Product>?, int)?>( null, false, "Failed to retrieve all Products from repository!" )
            : new DtoResponse<(IEnumerable<Product>?, int)?>( result, true, "Successfully retrieved all Products from repository." );
    }
    async Task<DtoResponse<(IEnumerable<Product>?, int)?>> SearchProducts( ProductSearchFilters_DTO searchFiltersDTO )
    {
        DtoResponse<ValidatedSearchFilters?> validatedFilters = await GetValidatedSearchFilters( searchFiltersDTO );

        if ( !validatedFilters.IsSuccessful() )
            return new DtoResponse<(IEnumerable<Product>?, int)?>( validatedFilters.Message );

        (IEnumerable<Product>?, int)? result = await _repository.SearchProducts( validatedFilters.Data! );

        return result is { Item1: not null, Item2: > 0 } ?
            new DtoResponse<(IEnumerable<Product>?, int)?>( null, false, "Failed to retrieve products search from repository!" ) :
            new DtoResponse<(IEnumerable<Product>?, int)?>( result!.Value!, true, "Successfully retrieved Products from repository." );
    }
    async Task<DtoResponse<ValidatedSearchFilters?>> GetValidatedSearchFilters( ProductSearchFilters_DTO searchFiltersDTO )
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
        //CATEGORY
        DtoResponse<int> categoryResult = await ValidateCategoryFilter( searchFiltersDTO.Category );
        if ( !categoryResult.IsSuccessful() )
            return new DtoResponse<ValidatedSearchFilters?>( categoryResult.Message );
        validatedFilters.Category = categoryResult.Data;
        // NO SPECS
        if ( searchFiltersDTO.SpecFilters == null )
            return new DtoResponse<ValidatedSearchFilters?>( validatedFilters, true, "Successfully validated the Product Filters." );
        // HAS SPECS
        Task<DtoResponse<CachedSpecDescrs?>> specsTask = null; //_specService.GetSpecsDTO();
        Task<DtoResponse<SpecValues_DTO?>> lookupsTask = null; //_specService.GetSpecLookupsDTO();

        await Task.WhenAll( specsTask, lookupsTask );

        if ( specsTask.Result.Data == null || !specsTask.Result.Success )
            return new DtoResponse<ValidatedSearchFilters?>( null, false, specsTask.Result.Message );
        if ( lookupsTask.Result.Data == null || !lookupsTask.Result.Success )
            return new DtoResponse<ValidatedSearchFilters?>( null, false, lookupsTask.Result.Message );

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
                if ( validatedFilters.Category != null && !spec.SpecCategoryIds.Contains( validatedFilters.Category.Value ) )
                    continue;
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

        return new DtoResponse<ValidatedSearchFilters?>( validatedFilters, true, "Successfully validated the product filters." );
    }

    async Task<DtoResponse<int>> ValidateCategoryFilter( string? categoryUrl )
    {
        if ( categoryUrl == null )
            return new DtoResponse<int>( -1, true, "Successfully validated CategoryFilter." );
        
        DtoResponse<Categories_DTO?> categories = await _categoryService.GetCategories();

        if ( !categories.IsSuccessful() )
            return new DtoResponse<int>( categories.Message );

        return !categories.Data!.CategoryIdsByUrl.TryGetValue( categoryUrl, out int categoryId ) 
            ? new DtoResponse<int>( "Invalid Category!" ) 
            : new DtoResponse<int>( categoryId, true, "Successfully validated CategoryFilter." );
    }

    static ProductDetails_DTO GetProductDetailsDTO( ProductDetails productDetails )
    {
        var DTO = new ProductDetails_DTO {
            ProductId = productDetails.Product.ProductId,
            ProductName = productDetails.Product.ProductName,
            ProductDescription = productDetails.ProductDescription.DescriptionBody
        };

        foreach ( ProductVariant variant in productDetails.ProductVariants ) {
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
    static async Task<Products_DTO> MapProductsToDto( IEnumerable<Product> models, int count )
    {
        var dto = new Products_DTO();

        await Task.Run( () =>
        {
            dto.Count = count;
            
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
    static async Task<ProductDetails_DTO> MapProductDetailsToDto( ProductDetails model )
    {
        var dto = new ProductDetails_DTO { };

        await Task.Run( () => { } );

        return dto;
    }
}