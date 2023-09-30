using BlazorElectronics.Shared.DataTransferObjects.Specs;

namespace BlazorElectronics.Server.Models.Specs;

public sealed class CachedSpecDescrs
{
    public Dictionary<int, Spec_DTO> SpecsById { get; set; } = new();
    public Dictionary<string, int> IdsByName { get; set; } = new();
    public Dictionary<int, List<int>> IdsByCategoryId { get; set; } = new();

    public bool MissingData() { return SpecsById == null || IdsByName == null || IdsByCategoryId == null; }
}