using BlazorElectronics.Shared.DtosOutbound.Products;
using Microsoft.Extensions.Caching.Distributed;

namespace BlazorElectronics.Server.Services.Products;

public sealed class ProductCache : CachedService, IProductCache
{
    const string CACHE_KEY_PRODUCT_DETAILS = "ProductDetails_";
    
    public ProductCache( IDistributedCache memoryCache ) : base( memoryCache ) { }
    
    public async Task<ProductDetails_DTO?> GetProductDetails( int id )
    {
        string key = $"{CACHE_KEY_PRODUCT_DETAILS}{id}";
        return await GetFromCache<ProductDetails_DTO>( key );
    }
    public async Task CacheProductDetails( ProductDetails_DTO dto )
    {
        string key = $"{CACHE_KEY_PRODUCT_DETAILS}{dto.Id}";
        await Cache( key, dto, new DistributedCacheEntryOptions()
            .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
            .SetAbsoluteExpiration( TimeSpan.FromDays( 1 ) ) );
    }
}