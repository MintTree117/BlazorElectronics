using BlazorElectronics.Server.Models.Specs;
using BlazorElectronics.Server.Repositories.Specs;
using Microsoft.Extensions.Caching.Distributed;

namespace BlazorElectronics.Server.Services.Specs;

public sealed class SpecCache : CachedService, ISpecCache
{
    const string CACHE_KEY_DESCRS = "Descrs";
    const string CACHE_KEY_LOOKUPS = "Lookups";

    HashSet<int> CategoryKeys = new();

    readonly DistributedCacheEntryOptions _specsCacheEntryOptions = new DistributedCacheEntryOptions()
        .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
        .SetAbsoluteExpiration( TimeSpan.FromHours( 5.0 ) );
    readonly DistributedCacheEntryOptions _lookupsCacheEntryOptions = new DistributedCacheEntryOptions()
        .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
        .SetAbsoluteExpiration( TimeSpan.FromHours( 5.0 ) );

    public SpecCache( IDistributedCache cache ) : base( cache ) { }

    public async Task<CachedSpecDescrs?> GetSpecDescrs( int categoryId )
    {
        string key = $"{CACHE_KEY_DESCRS}{categoryId}";
        return await GetFromCache<CachedSpecDescrs>( key );
    }
    public async Task<CachedSpecLookups?> GetSpecLookups( int categoryId )
    {
        string key = $"{CACHE_KEY_LOOKUPS}{categoryId}";
        return await GetFromCache<CachedSpecLookups>( key );
    }
    public async Task CacheSpecDescrs( CachedSpecDescrs dto, int categoryId )
    {
        string key = $"{CACHE_KEY_DESCRS}{categoryId}";
        await Cache( key, dto, new DistributedCacheEntryOptions()
            .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
            .SetAbsoluteExpiration( TimeSpan.FromDays( 1 ) ) );
    }
    public async Task CacheSpecLookups( CachedSpecLookups dto, int categoryId )
    {
        string key = $"{CACHE_KEY_LOOKUPS}{categoryId}";
        await Cache( key, dto, new DistributedCacheEntryOptions()
            .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
            .SetAbsoluteExpiration( TimeSpan.FromDays( 1 ) ) );
    }
}