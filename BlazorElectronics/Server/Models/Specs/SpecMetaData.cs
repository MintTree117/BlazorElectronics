using System.Collections.Concurrent;

namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecMetaData
{
    public ConcurrentDictionary<int, SpecDataType> _dataTypesById { get; set; } = new();
    public ConcurrentDictionary<int, List<int>> _specIdsByCategoryId { get; set; } = new();
    public ConcurrentDictionary<string, int> _specIdsByName { get; set; } = new();
    public ConcurrentDictionary<int, Spec> _specsById { get; set; } = new();
}