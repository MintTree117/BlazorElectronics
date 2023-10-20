using BlazorElectronics.Shared.DtosOutbound.Products;

namespace BlazorElectronics.Server.Services.Products;

public interface IProductCache
{
    Task<ProductsFeatured_DTO?> GetFeaturedProducts();
    Task<Products_DTO?> GetTopDeals();
    Task<ProductDetails_DTO?> GetProductDetails( int id );
    Task CacheFeaturedProducts( ProductsFeatured_DTO dto );
    Task CacheTopDeals( Products_DTO dto );
    Task CacheProductDetails( ProductDetails_DTO dto );
}