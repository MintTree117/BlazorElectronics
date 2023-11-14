using BlazorElectronics.Server.Services.Products;
using BlazorElectronics.Shared.DtosOutbound.Products;
using Microsoft.Extensions.Caching.Distributed;

namespace BlazorElectronics.Server.Caches.Products;

public sealed class ProductCache : ServiceCache, IProductCache
{
    const string CACHE_KEY_PRODUCT_DETAILS = "ProductDetails_";
    
    public ProductCache( IDistributedCache memoryCache ) : base( memoryCache ) { }
    
    public async Task<ProductDetailsResponse?> GetProductDetails( int id )
    {
        string key = $"{CACHE_KEY_PRODUCT_DETAILS}{id}";
        return await GetFromCache<ProductDetailsResponse>( key );
    }
    public async Task CacheProductDetails( ProductDetailsResponse response )
    {
        string key = $"{CACHE_KEY_PRODUCT_DETAILS}{response.Id}";
        await Cache( key, response, new DistributedCacheEntryOptions()
            .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
            .SetAbsoluteExpiration( TimeSpan.FromDays( 1 ) ) );
    }
}