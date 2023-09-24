using System.Collections.Concurrent;
using BlazorElectronics.Server.Models.Specs;

namespace BlazorElectronics.Server.Services.Specs;

public interface ISpecCache
{
    // SET
    Task CacheSpecDataTypes( List<SpecDataType> dataTypes );
    Task CacheSpecCategories( List<SpecCategory> specCategories );
    Task CacheSpecs( List<Spec> specs );
    Task CacheSpecLookups( List<SpecLookup> lookupValues );
    // GET
    Task<ConcurrentDictionary<int, List<string>>?> TryGetSpecIdsByCategoryId();
    Task<ConcurrentDictionary<int, SpecDataType>?> TryGetDataTypesById();
    Task<List<Spec>?> TryGetSpecs();
    Task<Dictionary<int, List<object>>?> TryGetSpecLookupsById();
}