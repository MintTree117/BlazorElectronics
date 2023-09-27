using System.Text.Json;
using BlazorElectronics.Server.Models.Specs;
using Microsoft.Extensions.Caching.Distributed;

namespace BlazorElectronics.Server.Repositories.Specs;

public sealed class CachedSpecRepository : CachedRepository, ISpecRepository
{
    const string CACHE_KEY_SPECS = "Specs";
    const string CACHE_KEY_LOOKUP_VALUES = "LookupValues";

    ISpecRepository _repository;

    public CachedSpecRepository( IDistributedCache cache, ISpecRepository repository ) : base( cache ) { _repository = repository; }
    
    public async Task CacheSpecLookups( List<SpecLookup> specLookups )
    {
        await Task.Run( () => {
            var _specValues = new Dictionary<int, List<object>>();
            foreach ( SpecLookup s in specLookups ) {
                if ( s.LookupValue == null )
                    continue;
                if ( !_specValues.ContainsKey( s.SpecId ) )
                    _specValues.Add( s.SpecId, new List<object>() );
                _specValues[ s.SpecId ].Add( s.LookupValue );
            }
            var bytes = JsonSerializer.SerializeToUtf8Bytes( _specValues );
            MemoryCache.SetAsync( CACHE_KEY_LOOKUP_VALUES, bytes, new DistributedCacheEntryOptions()
                .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
                .SetAbsoluteExpiration( TimeSpan.FromHours( 5.0 ) ) );
        } );
    }
    public async Task<Dictionary<string, Spec>?> GetSpecs()
    {
        return await Get( CACHE_KEY_SPECS, _repository.GetSpecs(), new DistributedCacheEntryOptions()
            .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
            .SetAbsoluteExpiration( TimeSpan.FromHours( 5.0 ) ) );
    }
    public async Task<Dictionary<int, List<object>>?> GetSpecLookups()
    {
        return await Get( CACHE_KEY_SPECS, _repository.GetSpecLookups(), new DistributedCacheEntryOptions()
            .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
            .SetAbsoluteExpiration( TimeSpan.FromHours( 5.0 ) ) );
    }
}