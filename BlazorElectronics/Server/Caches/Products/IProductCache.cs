using BlazorElectronics.Shared.DtosOutbound.Products;

namespace BlazorElectronics.Server.Caches.Products;

public interface IProductCache
{
    Task<ProductDetails_DTO?> GetProductDetails( int id );
    Task CacheProductDetails( ProductDetails_DTO dto );
}