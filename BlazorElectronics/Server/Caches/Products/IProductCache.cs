using BlazorElectronics.Shared.DtosOutbound.Products;

namespace BlazorElectronics.Server.Caches.Products;

public interface IProductCache
{
    Task<ProductDetailsResponse?> GetProductDetails( int id );
    Task CacheProductDetails( ProductDetailsResponse response );
}