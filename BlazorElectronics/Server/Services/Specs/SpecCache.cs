using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using BlazorElectronics.Server.Models.Specs;

namespace BlazorElectronics.Server.Services.Specs;

public sealed class SpecCache : CacheService, ISpecCache
{
    const string LOOKUP_SPEC_VALUES_KEY = "LookupValues";

    ConcurrentDictionary<int, SpecDataDescr>? _dataTypesById;
    ConcurrentDictionary<int, List<int>>? _specIdsByCategoryId;
    ConcurrentDictionary<string, int>? _specIdsByName;
    ConcurrentDictionary<int, Spec>? _specsById;

    public SpecCache( IMemoryCache cache ) : base( cache ) { }

    // SET
    public async Task CacheSpecDataTypes( List<SpecDataDescr> dataTypes )
    {
        await Task.Run( () => {
            _dataTypesById = new ConcurrentDictionary<int, SpecDataDescr>();
            foreach ( SpecDataDescr d in dataTypes )
                _dataTypesById.TryAdd( d.DataTypeId, d );
        } );
    }
    public async Task CacheSpecCategories( List<SpecCategory> specCategories )
    {
        await Task.Run( () => {
            _specIdsByCategoryId = new ConcurrentDictionary<int, List<int>>();
            foreach ( SpecCategory c in specCategories ) {
                if ( !_specIdsByCategoryId.ContainsKey( c.CategoryId ) )
                    _specIdsByCategoryId.TryAdd( c.CategoryId, new List<int>() );
                _specIdsByCategoryId[ c.CategoryId ].Add( c.SpecId );
            }
        } );
    }
    public async Task CacheSpecs( List<Spec> specs )
    {
        await Task.Run( () => {
            _specIdsByName = new ConcurrentDictionary<string, int>();
            _specsById = new ConcurrentDictionary<int, Spec>();
            foreach ( Spec s in specs ) {
                if ( string.IsNullOrEmpty( s.SpecName ) )
                    continue;
                _specIdsByName.TryAdd( s.SpecName, s.SpecId );
                _specsById.TryAdd( s.SpecId, s );
            }
        } );
    }
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
            MemoryCache.Set( LOOKUP_SPEC_VALUES_KEY, _specValues, new MemoryCacheEntryOptions()
                .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
                .SetAbsoluteExpiration( TimeSpan.FromHours( 5.0 ) ) );
        } );
    }
    
    // GET
    public ConcurrentDictionary<int, SpecDataDescr>? TryGetDataTypesById()
    {
        return _dataTypesById;
    }
    public ConcurrentDictionary<int, List<int>>? TryGetSpecIdsByCategoryId()
    {
        return _specIdsByCategoryId;
    }
    public ConcurrentDictionary<string, int>? TryGetSpecIdsByName()
    {
        return _specIdsByName;
    }
    public ConcurrentDictionary<int, Spec>? TryGetSpecsById()
    {
        return _specsById;
    }

    public async Task<Dictionary<int, List<object>>?> TryGetSpecLookupsById()
    {
        Dictionary<int, List<object>>? lookupValues = null;
        await Task.Run( () => { MemoryCache.TryGetValue( LOOKUP_SPEC_VALUES_KEY, out lookupValues ); } );
        return lookupValues;
    }
}