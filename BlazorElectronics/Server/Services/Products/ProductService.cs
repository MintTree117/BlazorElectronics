using BlazorElectronics.Server.Caches.Products;
using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Server.Repositories;
using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Server.Services.Products;

public class ProductService : IProductService
{
    readonly IProductCache _productCache;
    readonly IProductRepository _productRepository;

    public ProductService( IProductCache productCache, IProductRepository productRepository )
    {
        _productCache = productCache;
        _productRepository = productRepository;
    }

    public async Task<ServiceResponse<List<Product_DTO>?>> GetProducts()
    {
        IEnumerable<Product>? products = await _productRepository.GetProducts();

        if ( products == null )
            return new ServiceResponse<List<Product_DTO>?>( null, false, "Failed to retrieve products from database!" );

        var productDtos = new List<Product_DTO>();

        await Task.Run( () => {
            foreach ( Product p in products ) {
                productDtos.Add( new Product_DTO {
                    Id = p.ProductId,
                    Title = p.ProductName,
                    Thumbnail = p.ProductThumbnail,
                    Rating = p.ProductRating
                } );
            }
        } );

        return new ServiceResponse<List<Product_DTO>?>( productDtos, true, "Successfully retrieved product Dto's from repository." );
    }
    public async Task<ServiceResponse<ProductDetails_DTO?>> GetProductDetails()
    {
        ProductDetails productDetails = await _productRepository.GetProductDetails();

        return productDetails == null
            ? new ServiceResponse<ProductDetails_DTO?>( null, false, "Failed to retrieve product details from database!" )
            : new ServiceResponse<ProductDetails_DTO?>( GetProductDetailsDTO( productDetails ), true, "Successfully retrieved product details from the database." );
    }

    static ProductDetails_DTO GetProductDetailsDTO( ProductDetails productDetails )
    {
        var DTO = new ProductDetails_DTO();

        DTO.ProductId = productDetails.Product.ProductId;
        DTO.ProductName = productDetails.Product.ProductName;
        DTO.ProductDescription = productDetails.ProductDescription.DescriptionBody;

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