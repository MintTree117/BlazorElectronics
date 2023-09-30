namespace BlazorElectronics.Shared.DataTransferObjects.Specs;

public sealed class SpecValues_DTO
{
    public Dictionary<int, Dictionary<int, object>> LookupValuesBySpecId { get; set; } = new();
    public Dictionary<int, Dictionary<int, object>> DynamicFiltersValuesBySpecId { get; set; } = new();
    public Dictionary<int, Dictionary<int, SpecDynamicValueLimits_DTO>> DynamicLimitsBySpecId { get; set; } = new();
}