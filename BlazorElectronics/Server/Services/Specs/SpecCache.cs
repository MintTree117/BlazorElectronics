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

    ISpecRepository _repository;

    public SpecCache( IDistributedCache cache, ISpecRepository repository ) : base( cache ) { _repository = repository; }

    public async Task<Specs_DTO?> GetSpecs() { return await GetFromCache<Specs_DTO>( CACHE_KEY_SPECS ); }
    public async Task<SpecLookups_DTO?> GetSpecLookups() { return await GetFromCache<SpecLookups_DTO>( CACHE_KEY_LOOKUP_VALUES ); }
    public async Task CacheSpecs( Specs_DTO dto )
    {
        await Cache( CACHE_KEY_SPECS, dto, new DistributedCacheEntryOptions()
            .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
            .SetAbsoluteExpiration( TimeSpan.FromDays( 1 ) ) );
    }
    public async Task CacheSpecLookups( SpecLookups_DTO dto )
    {
        await Cache( CACHE_KEY_LOOKUP_VALUES, dto, new DistributedCacheEntryOptions()
            .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
            .SetAbsoluteExpiration( TimeSpan.FromDays( 1 ) ) );
    }
}