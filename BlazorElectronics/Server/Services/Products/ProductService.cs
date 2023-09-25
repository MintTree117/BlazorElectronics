using System.Collections.Concurrent;
using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Server.Models.Specs;
using BlazorElectronics.Server.Repositories.Products;
using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Server.Services.Specs;
using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Server.Services.Products;

public class ProductService : IProductService
{
    readonly ISpecService _specService;
    readonly ICategoryService _categoryService;
    readonly IProductCache _productCache;
    readonly IProductRepository _productRepository;

    const int MAX_PRODUCT_LIST_ROWS = 100;

    public ProductService( ICategoryService categoryService, IProductCache productCache, IProductRepository productRepository, ISpecService specService )
    {
        _categoryService = categoryService;
        _productCache = productCache;
        _productRepository = productRepository;
        _specService = specService;
    }
    
    public async Task<ServiceResponse<string>> TestGetQueryString( ProductSearchFilters_DTO filters )
    {
        string query = await _productRepository.TEST_GET_QUERY_STRING( new ValidatedSearchFilters() );
        return new ServiceResponse<string?>( query, true, "Test query" );
    }
    public async Task<ServiceResponse<Products_DTO?>> GetProducts( ProductSearchFilters_DTO? searchFilters = null )
    {
        ServiceResponse<IEnumerable<Product>> getResult = searchFilters == null
            ? await GetAllProducts()
            : await SearchProducts( searchFilters );

        if ( getResult?.Data == null )
            return new ServiceResponse<Products_DTO?>( null, false, "Failed to retrieve products from database!" );

        var productList = new Products_DTO();

        await Task.Run( () => {
            foreach ( Product p in getResult.Data ) {
                var productDto = new Product_DTO {
                    Id = p.ProductId,
                    Title = p.ProductName,
                    Thumbnail = p.ProductThumbnail,
                    Rating = p.ProductRating
                };
                foreach ( ProductVariant v in p.ProductVariants ) {
                    productDto.Variants.Add( new ProductVariant_DTO {
                        VariantId = v.VariantId,
                        VariantName = v.VariantName,
                        Price = v.VariantPriceMain,
                        SalePrice = v.VariantPriceSale
                    } );
                }
                productList.Products.Add( productDto );
            }
        } );

        return new ServiceResponse<Products_DTO?>( productList, true, "Successfully retrieved product Dto's from repository." );
    }
    public async Task<ServiceResponse<ProductDetails_DTO?>> GetProductDetails( int productId )
    {
        ProductDetails productDetails = await _productRepository.GetProductDetails( productId );

        return productDetails == null
            ? new ServiceResponse<ProductDetails_DTO?>( null, false, "Failed to retrieve product details from database!" )
            : new ServiceResponse<ProductDetails_DTO?>( GetProductDetailsDTO( productDetails ), true, "Successfully retrieved product details from the database." );
    }

    async Task<ServiceResponse<IEnumerable<Product>>> GetAllProducts()
    {
        IEnumerable<Product>? result = await _productRepository.GetAllProducts();

        return result == null
            ? new ServiceResponse<IEnumerable<Product>>( null, false, "Failed to retrieve products from database!" )
            : new ServiceResponse<IEnumerable<Product>>( result, true, "Successfully retrieved products from database." );
    }
    async Task<ServiceResponse<IEnumerable<Product>>> SearchProducts( ProductSearchFilters_DTO filters )
    {
        var validatedFilters = new ValidatedSearchFilters();
        
        validatedFilters.Page = Math.Max( filters.Page, 0 );
        validatedFilters.Rows = Math.Max( filters.Rows, 0 );
        validatedFilters.Rows = Math.Min( filters.Rows, MAX_PRODUCT_LIST_ROWS );
        validatedFilters.MinPrice = filters.MinPrice < 0 ? null : Math.Max( filters.MinPrice, 0 );
        validatedFilters.MaxPrice = filters.MaxPrice < 0 ? null : Math.Max( filters.MaxPrice, 0 );
        validatedFilters.MinRating = filters.MinRating < 0 ? null : Math.Max( filters.MinRating, 0 );
        validatedFilters.MaxRating = filters.MaxRating < 0 ? null : Math.Max( filters.MaxRating, 0 );
        validatedFilters.SearchText = string.IsNullOrEmpty( filters.SearchText ) ? null : filters.SearchText;

        if ( !string.IsNullOrEmpty( filters.Category ) ) {
            ServiceResponse<int> validatedCategory = await _categoryService.CategoryIdFromUrl( filters.Category );
            if ( validatedCategory.Success )
                validatedFilters.Category = validatedCategory.Data;
        }

        if ( filters.SpecFilters != null ) {
            Task<ServiceResponse<SpecMetaData>> metaTask = _specService.GetSpecMetaData();
            Task<ServiceResponse<Dictionary<int, List<object>>>> lookupTask = _specService.GetSpecLookups();

            await Task.WhenAll( metaTask, lookupTask );

            if ( metaTask.Result.Data == null || !metaTask.Result.Success )
                return new ServiceResponse<IEnumerable<Product>>( null, false, metaTask.Result.Message );
            if ( lookupTask.Result.Data == null || !lookupTask.Result.Success )
                return new ServiceResponse<IEnumerable<Product>>( null, false, lookupTask.Result.Message );

            SpecMetaData specMeta = metaTask.Result.Data;
            Dictionary<int, List<object>> specLookup = lookupTask.Result.Data;
            
            foreach ( ProductSpecFilter_DTO specFilter in filters.SpecFilters ) {
                if ( string.IsNullOrEmpty( specFilter.SpecName ) )
                    continue;
                if ( specFilter.SpecValue == null )
                    continue;
                if ( !specMeta._specIdsByName.TryGetValue( specFilter.SpecName, out int specId ) )
                    continue;
                if ( !specMeta._specsById.TryGetValue( specId, out Spec? spec ) || spec == null )
                    continue;
                if ( validatedFilters.Category != null && !specMeta._specIdsByCategoryId.TryGetValue( validatedFilters.Category.Value, out List<int>? specsOfCategory ) )
                    continue;
                if ( !Enum.IsDefined( typeof( SpecType ), specFilter.SpecType ) )
                    continue;
                if (!Enum.IsDefined( typeof( FilterType ), specFilter.FilterType ))
                    continue;
                if ( specFilter.SpecType == ( int ) SpecType.Lookup
                     || !specLookup.TryGetValue( specId, out List<object>? lookupValues )
                     || !TryValidateLookupFilter( specFilter.SpecValue, ( FilterType ) specFilter.FilterType ) )
                    continue;
                validatedFilters.LookupSpecFilters.Add( new SpecFilter {
                    SpecName = specFilter.SpecName,
                    SpecValue = specFilter.SpecValue
                } );
            }
        }
        
        IEnumerable<Product> products = await _productRepository.SearchProducts( validatedFilters );
        return new ServiceResponse<IEnumerable<Product>>( products, true, "Repo Message" );
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
    static bool TryValidateLookupFilter( object value, FilterType filterType )
    {
        return false;
    }
    static bool TryValidateRawFilter()
    {
        return false;
    }
}