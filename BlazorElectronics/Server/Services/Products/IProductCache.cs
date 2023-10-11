using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Server.Services.Products;

public interface IProductCache
{
    Task<ProductsFeatured_DTO?> GetFeaturedProducts();
    Task<ProductDetails_DTO?> GetProductDetails( int id );
    Task CacheFeaturedProducts( ProductsFeatured_DTO dto );
    Task CacheProductDetails( ProductDetails_DTO dto );
}