using BlazorElectronics.Server.Caches;
using BlazorElectronics.Shared.Outbound.Features;
using Microsoft.Extensions.Caching.Distributed;

namespace BlazorElectronics.Server.Services.Features;

public class FeaturesCache : ServiceCache, IFeaturesCache
{
    const string CACHE_KEY_FEATURED_PRODUCTS = "FeaturedProducts";
    const string CACHE_KEY_FEATURED_DEALS = "FeaturedDeals";

    public FeaturesCache( IDistributedCache memoryCache ) : base( memoryCache ) { }
    
    public async Task<FeaturedProducts_DTO?> GetFeaturedProducts()
    {
        try
        {
            return await GetFromCache<FeaturedProducts_DTO>( CACHE_KEY_FEATURED_PRODUCTS );
        }
        catch ( Exception e )
        {
            throw new ServiceException( e.Message, e );
        }
    }
    public async Task<FeaturedDeals_DTO?> GetFeaturedDeals()
    {
        try
        {
            return await GetFromCache<FeaturedDeals_DTO>( CACHE_KEY_FEATURED_DEALS );   
        }
        catch ( Exception e )
        {
            throw new ServiceException( e.Message, e );
        }
    }
    public async Task CacheFeaturedProducts( FeaturedProducts_DTO dto )
    {
        try
        {
            await Cache( CACHE_KEY_FEATURED_PRODUCTS, dto, new DistributedCacheEntryOptions()
                .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
                .SetAbsoluteExpiration( TimeSpan.FromDays( 1 ) ) );   
        }
        catch ( Exception e )
        {
            throw new ServiceException( e.Message, e );
        }
    }
    public async Task CacheFeaturedDeals( FeaturedDeals_DTO dto )
    {
        try
        {
            await Cache( CACHE_KEY_FEATURED_DEALS, dto, new DistributedCacheEntryOptions()
                .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
                .SetAbsoluteExpiration( TimeSpan.FromDays( 1 ) ) );   
        }
        catch ( Exception e )
        {
            throw new ServiceException( e.Message, e );
        }
    }
}