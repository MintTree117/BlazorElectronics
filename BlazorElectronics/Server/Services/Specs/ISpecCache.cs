using System.Collections.Concurrent;
using BlazorElectronics.Server.Models.Specs;

namespace BlazorElectronics.Server.Services.Specs;

public interface ISpecCache
{
    // SET
    Task CacheSpecDataTypes( List<SpecDataDescr> dataTypes );
    Task CacheSpecCategories( List<SpecCategory> specCategories );
    Task CacheSpecs( List<Spec> specs );
    Task CacheSpecLookups( List<SpecLookup> specLookups );
    // GET
    ConcurrentDictionary<int, SpecDataDescr>? TryGetDataTypesById();
    ConcurrentDictionary<int, List<int>>? TryGetSpecIdsByCategoryId();
    ConcurrentDictionary<string, int>? TryGetSpecIdsByName();
    ConcurrentDictionary<int, Spec>? TryGetSpecsById();
    Task<Dictionary<int, List<object>>?> TryGetSpecLookupsById();
}