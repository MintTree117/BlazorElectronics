using BlazorElectronics.Shared.DataTransferObjects.Products;
using Microsoft.Extensions.Caching.Distributed;

namespace BlazorElectronics.Server.Services.Products;

public sealed class ProductCache : CachedService, IProductCache
{
    const string CACHE_KEY_FEATURED_PRODUCTS = "FeaturedProducts";
    const string CACHE_KEY_PRODUCT_DETAILS = "ProductDetails_";
    
    public ProductCache( IDistributedCache memoryCache ) : base( memoryCache ) { }
    
    public async Task<ProductsFeatured_DTO?> GetFeaturedProducts()
    {
        return await GetFromCache<ProductsFeatured_DTO>( CACHE_KEY_FEATURED_PRODUCTS );
    }
    public async Task<ProductDetails_DTO?> GetProductDetails( int id )
    {
        string key = $"{CACHE_KEY_PRODUCT_DETAILS}{id}";
        return await GetFromCache<ProductDetails_DTO>( key );
    }
    public async Task CacheFeaturedProducts( ProductsFeatured_DTO dto )
    {
        await Cache( CACHE_KEY_FEATURED_PRODUCTS, dto, new DistributedCacheEntryOptions()
            .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
            .SetAbsoluteExpiration( TimeSpan.FromDays( 1 ) ) );
    }
    public async Task CacheProductDetails( ProductDetails_DTO dto )
    {
        string key = $"{CACHE_KEY_PRODUCT_DETAILS}{dto.ProductId}";
        await Cache( key, dto, new DistributedCacheEntryOptions()
            .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
            .SetAbsoluteExpiration( TimeSpan.FromDays( 1 ) ) );
    }
}