using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Server.Models.Specs;
using BlazorElectronics.Server.Repositories.Products;
using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Server.Services.Specs;
using BlazorElectronics.Shared.DataTransferObjects.Products;
using BlazorElectronics.Shared.DataTransferObjects.Specs;

namespace BlazorElectronics.Server.Services.Products;

public class ProductService : IProductService
{
    readonly ISpecService _specService;
    readonly ICategoryService _categoryService;
    readonly IProductCache _productCache;
    readonly IProductRepository _repository;

    const int MAX_PRODUCT_LIST_ROWS = 100;

    public ProductService( ICategoryService categoryService, IProductCache productCache, IProductRepository repository, ISpecService specService )
    {
        _categoryService = categoryService;
        _productCache = productCache;
        _repository = repository;
        _specService = specService;
    }
    
    public async Task<ServiceResponse<string>> TestGetQueryString( ProductSearchFilters_DTO filters )
    {
        string query = await _repository.TEST_GET_QUERY_STRING( new ValidatedSearchFilters() );
        return new ServiceResponse<string?>( query, true, "Test query" );
    }
    public async Task<ServiceResponse<Products_DTO?>> GetProducts( ProductSearchFilters_DTO? searchFilters = null )
    {
        ServiceResponse<(IEnumerable<Product>?, int)?> getResult;

        if ( searchFilters == null )
            getResult = await GetAllProducts();
        else
        {
            ServiceResponse<ValidatedSearchFilters> validatedFilters = await GetValidatedSearchFilters( searchFilters );
            if ( validatedFilters.Data == null || !validatedFilters.Success )
                return new ServiceResponse<Products_DTO?>( null, false, validatedFilters.Message );
            getResult = await SearchProducts( validatedFilters.Data );
        }

        if ( getResult.Data == null || !getResult.Success )
            return new ServiceResponse<Products_DTO?>( null, false, getResult.Message );

        Products_DTO productsDto = await MapProductsToDto( getResult.Data.Value.Item1!, getResult.Data.Value.Item2 );
        
        return new ServiceResponse<Products_DTO?>( productsDto, true, "Successfully retrieved product Dto's from repository." );
    }
    public async Task<ServiceResponse<ProductDetails_DTO?>> GetProductDetails( int productId )
    {
        ProductDetails_DTO? dto = await _productCache.GetProductDetails( productId );

        if ( dto != null )
            return new ServiceResponse<ProductDetails_DTO?>( dto, true, "Success. Retrieved ProductDetails_DTO from cache." );

        ProductDetails? model = await _repository.GetProductDetails( productId );

        if ( model == null )
            return new ServiceResponse<ProductDetails_DTO?>( null, false, "Failed to retrieve ProductDetails_DTO from cache, and ProductDetails from repository!" );

        dto = await MapProductDetailsToDto( model );
        await _productCache.CacheProductDetails( dto );

        return new ServiceResponse<ProductDetails_DTO?>( dto, true, "Successfully retrieved ProductDetails from repository, mapped to DTO, and cached." );
    }

    async Task<ServiceResponse<(IEnumerable<Product>?, int)?>> GetAllProducts()
    {
        (IEnumerable<Product>?, int)? result = await _repository.GetAllProducts();
        bool success = result is { Item1: not null, Item2: > 0 };
        return result == null
            ? new ServiceResponse<(IEnumerable<Product>?, int)?>( null, false, "Failed to retrieve products from database!" )
            : new ServiceResponse<(IEnumerable<Product>?, int)?>( result, true, "Successfully retrieved products from database." );
    }
    async Task<ServiceResponse<(IEnumerable<Product>?, int)?>> SearchProducts( ValidatedSearchFilters searchFiltersDTO )
    {
        (IEnumerable<Product>?, int)? result = await _repository.SearchProducts( searchFiltersDTO );
        bool success = result is { Item1: not null, Item2: > 0 };
        return success ?
            new ServiceResponse<(IEnumerable<Product>?, int)?>( null, false, "Failed to retrieve products search from repository!" ) :
            new ServiceResponse<(IEnumerable<Product>?, int)?>( result!.Value!, true, "Repo Message" );
    }
    async Task<ServiceResponse<ValidatedSearchFilters>> GetValidatedSearchFilters( ProductSearchFilters_DTO searchFiltersDTO )
    {
        var validatedFilters = new ValidatedSearchFilters();

        validatedFilters.Page = Math.Max( searchFiltersDTO.Page, 0 );
        validatedFilters.Rows = Math.Max( searchFiltersDTO.Rows, 0 );
        validatedFilters.Rows = Math.Min( searchFiltersDTO.Rows, MAX_PRODUCT_LIST_ROWS );
        validatedFilters.MinPrice = searchFiltersDTO.MinPrice < 0 ? null : Math.Max( searchFiltersDTO.MinPrice, 0 );
        validatedFilters.MaxPrice = searchFiltersDTO.MaxPrice < 0 ? null : Math.Max( searchFiltersDTO.MaxPrice, 0 );
        validatedFilters.MinRating = searchFiltersDTO.MinRating < 0 ? null : Math.Max( searchFiltersDTO.MinRating, 0 );
        validatedFilters.MaxRating = searchFiltersDTO.MaxRating < 0 ? null : Math.Max( searchFiltersDTO.MaxRating, 0 );
        validatedFilters.SearchText = string.IsNullOrEmpty( searchFiltersDTO.SearchText ) ? null : searchFiltersDTO.SearchText;

        if ( !string.IsNullOrEmpty( searchFiltersDTO.Category ) )
        {
            ServiceResponse<int> validatedCategory = null;// await _categoryService.CategoryIdFromUrl( searchFiltersDTO.Category );
            if ( validatedCategory.Success )
                validatedFilters.Category = validatedCategory.Data;
        }

        if ( searchFiltersDTO.SpecFilters == null )
            return new ServiceResponse<ValidatedSearchFilters>( validatedFilters, true, "Successfully validated the product filters." );
        
        Task<ServiceResponse<Specs_DTO?>> specsTask = _specService.GetSpecsDTO();
        Task<ServiceResponse<SpecLookups_DTO?>> lookupsTask = _specService.GetSpecLookupsDTO();

        await Task.WhenAll( specsTask, lookupsTask );

        if ( specsTask.Result.Data == null || !specsTask.Result.Success )
            return new ServiceResponse<ValidatedSearchFilters>( null, false, specsTask.Result.Message );
        if ( lookupsTask.Result.Data == null || !lookupsTask.Result.Success )
            return new ServiceResponse<ValidatedSearchFilters>( null, false, lookupsTask.Result.Message );

        Specs_DTO? specs = specsTask.Result.Data;
        SpecLookups_DTO? lookups = lookupsTask.Result.Data;

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
                if ( !Enum.IsDefined( typeof( SpecType ), specFilterDTO.SpecType ) )
                    continue;
                if ( !Enum.IsDefined( typeof( SpecFilterType ), specFilterDTO.FilterType ) )
                    continue;
                if ( !ValidateSpecLookup( specId, specFilterDTO, lookups ) )
                    continue;
                validatedFilters.LookupSpecFilters.Add( new ProductSpecFilter {
                    SpecName = specFilterDTO.SpecName,
                    SpecValue = specFilterDTO.SpecValue,
                    DataType = ( SpecDataType ) specFilterDTO.DataType,
                    FilterType = ( SpecFilterType ) specFilterDTO.FilterType,
                    SpecType = ( SpecType ) specFilterDTO.SpecType
                } );
            }
        } );

        return new ServiceResponse<ValidatedSearchFilters>( validatedFilters, true, "Successfully validated the product filters." );
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
    static bool ValidateSpecLookup( int specId, ProductSpecFilter_DTO specFilterDTO, SpecLookups_DTO specLookup )
    {
        if ( specFilterDTO.SpecType != ( int ) SpecType.Lookup )
            return true;
        return specLookup.LookupValuesBySpecId.TryGetValue( specId, out List<object>? values )
               && values.Any( o => specFilterDTO.SpecValue == o );
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