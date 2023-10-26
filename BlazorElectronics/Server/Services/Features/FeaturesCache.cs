using BlazorElectronics.Shared.Outbound.Features;
using Microsoft.Extensions.Caching.Distributed;

namespace BlazorElectronics.Server.Services.Features;

public class FeaturesCache : CachedService, IFeaturesCache
{
    const string CACHE_KEY_FEATURED_PRODUCTS = "FeaturedProducts";
    const string CACHE_KEY_FEATURED_DEALS = "FeaturedDeals";

    public FeaturesCache( IDistributedCache memoryCache ) : base( memoryCache ) { }
    
    public async Task<FeaturedProducts_DTO?> GetFeaturedProducts()
    {
        return await GetFromCache<FeaturedProducts_DTO>( CACHE_KEY_FEATURED_PRODUCTS );
    }
    public async Task<FeaturedDeals_DTO?> GetFeaturedDeals()
    {
        return await GetFromCache<FeaturedDeals_DTO>( CACHE_KEY_FEATURED_DEALS );
    }
    public async Task CacheFeaturedProducts( FeaturedProducts_DTO dto )
    {
        await Cache( CACHE_KEY_FEATURED_PRODUCTS, dto, new DistributedCacheEntryOptions()
            .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
            .SetAbsoluteExpiration( TimeSpan.FromDays( 1 ) ) );
    }
    public async Task CacheFeaturedDeals( FeaturedDeals_DTO dto )
    {
        await Cache( CACHE_KEY_FEATURED_DEALS, dto, new DistributedCacheEntryOptions()
            .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
            .SetAbsoluteExpiration( TimeSpan.FromDays( 1 ) ) );
    }
}