using System.Collections.Concurrent;
using BlazorElectronics.Server.Models.Specs;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorElectronics.Server.Services.Specs;

public sealed class SpecCache : CacheService, ISpecCache
{
    const string LOOKUP_SPEC_CATEGORIES_KEY = "LookupSpecCategories";
    const string RAW_SPECS_CATEGORIES_KEY = "RawSpecCategories";
    const string LOOKUP_SPECS_KEY = "LookupSpecs";
    const string RAW_SPECS_KEY = "RawSpecs";
    const string LOOKUP_SPEC_VALUES_KEY = "LookupValues";
    const string RAW_SPEC_VALUES_KEY = "RawValues";

    ConcurrentDictionary<int, SpecDataType>? _dataTypesById;
    ConcurrentDictionary<int, List<int>>? _specIdsByCategoryId;
    ConcurrentDictionary<string, int>? _specIdsByName;
    ConcurrentDictionary<int, Spec>? _specsById;
    ConcurrentDictionary<int, List<object>>? _specLookupsById;
    // RAW SPEC VALUES ARE CACHED USING IMEMORYCACHE!!!!!!!!

    public SpecCache( IMemoryCache cache ) : base( cache ) { }
    
    public Task CacheSpecDataTypes( List<SpecDataType> dataTypes ) { throw new NotImplementedException(); }
    public Task CacheSpecCategories( List<SpecCategory> specCategories ) { throw new NotImplementedException(); }
    public Task CacheSpecs( List<Spec> specs ) { throw new NotImplementedException(); }
    public Task CacheSpecLookups( List<SpecLookup> lookupValues ) { throw new NotImplementedException(); }
    public Task<ConcurrentDictionary<int, List<string>>?> TryGetSpecIdsByCategoryId() { throw new NotImplementedException(); }
    public Task<ConcurrentDictionary<int, SpecDataType>?> TryGetDataTypesById() { throw new NotImplementedException(); }
    public Task<List<Spec>?> TryGetSpecs() { throw new NotImplementedException(); }
    public Task<Dictionary<int, List<object>>?> TryGetSpecLookupsById() { throw new NotImplementedException(); }
}