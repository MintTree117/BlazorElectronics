namespace BlazorElectronics.Shared.DataTransferObjects.Specs;

public sealed class SpecFilters_DTO
{
    public List<SpecFilter_DTO> Filters { get; set; } = new();
    public Dictionary<string, int> IndicesByName { get; set; } = new();
}