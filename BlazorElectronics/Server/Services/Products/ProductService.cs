using BlazorElectronics.Server.Caches.Products;
using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Server.Repositories.Products;
using BlazorElectronics.Server.Services.Categories;
using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Server.Services.Products;

public class ProductService : IProductService
{
    readonly ICategoryService _categoryService;
    readonly IProductCache _productCache;
    readonly IProductRepository _productRepository;

    public ProductService( ICategoryService categoryService, IProductCache productCache, IProductRepository productRepository )
    {
        _categoryService = categoryService;
        _productCache = productCache;
        _productRepository = productRepository;
    }

    public async Task<ServiceResponse<ProductList_DTO?>> GetProducts( ProductSearchFilters_DTO? searchFilters = null )
    {
        ServiceResponse<IEnumerable<Product>> getResult = searchFilters == null
            ? await GetAllProducts()
            : await SearchProducts( searchFilters );

        if ( getResult?.Data == null )
            return new ServiceResponse<ProductList_DTO?>( null, false, "Failed to retrieve products from database!" );

        var productList = new ProductList_DTO();

        await Task.Run( () => {
            foreach ( Product p in getResult.Data ) {
                productList.Products.Add( new Product_DTO {
                    Id = p.ProductId,
                    Title = p.ProductName,
                    Thumbnail = p.ProductThumbnail,
                    Rating = p.ProductRating
                } );
            }
        } );

        return new ServiceResponse<ProductList_DTO?>( productList, true, "Successfully retrieved product Dto's from repository." );
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
        return null;
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
                VariantPriceMain = variant.VariantPriceMain,
                VariantPriceSale = variant.VariantPriceSale,
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
}