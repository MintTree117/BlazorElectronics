using BlazorElectronics.Server.Models.Specs;
using BlazorElectronics.Server.Repositories.Specs;
using BlazorElectronics.Shared.DataTransferObjects.Specs;
using Microsoft.Extensions.Caching.Distributed;

namespace BlazorElectronics.Server.Services.Specs;

public sealed class SpecCache : CachedService, ISpecCache
{
    const string CACHE_KEY_SPECS = "Specs";
    const string CACHE_KEY_LOOKUP_VALUES = "LookupValues";

    readonly DistributedCacheEntryOptions _specsCacheEntryOptions = new DistributedCacheEntryOptions()
        .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
        .SetAbsoluteExpiration( TimeSpan.FromHours( 5.0 ) );
    readonly DistributedCacheEntryOptions _lookupsCacheEntryOptions = new DistributedCacheEntryOptions()
        .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
        .SetAbsoluteExpiration( TimeSpan.FromHours( 5.0 ) );

    ISpecDescrRepository _descrRepository;

    public SpecCache( IDistributedCache cache, ISpecDescrRepository descrRepository ) : base( cache ) { _descrRepository = descrRepository; }

    public async Task<CachedSpecDescrs?> GetSpecDescrs() { return await GetFromCache<CachedSpecDescrs>( CACHE_KEY_SPECS ); }
    public async Task<CachedSpecLookups?> GetSpecLookups() { return await GetFromCache<CachedSpecLookups>( CACHE_KEY_LOOKUP_VALUES ); }
    public async Task CacheSpecDescrs( CachedSpecDescrs dto )
    {
        await Cache( CACHE_KEY_SPECS, dto, new DistributedCacheEntryOptions()
            .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
            .SetAbsoluteExpiration( TimeSpan.FromDays( 1 ) ) );
    }
    public async Task CacheSpecLookups( CachedSpecLookups dto )
    {
        await Cache( CACHE_KEY_LOOKUP_VALUES, dto, new DistributedCacheEntryOptions()
            .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
            .SetAbsoluteExpiration( TimeSpan.FromDays( 1 ) ) );
    }
}