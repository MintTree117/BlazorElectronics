using System.Collections.Concurrent;
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

    ConcurrentDictionary<int, List<int>> CategoryIdToLookupSpecIds;
    ConcurrentDictionary<int, List<int>> CategoryIdToRawSpecIds;

    public SpecCache( IMemoryCache cache ) : base( cache ) { }
}