namespace BlazorElectronics.Shared.Outbound.Specs;

public sealed class SpecFiltersResponse
{
    public List<SpecFilterTableResponse> IntFilters { get; set; } = new();
    public List<SpecFilterTableResponse> StringFilters { get; set; } = new();
    public List<string> BoolFilters { get; set; } = new();
    public List<SpecFilterTableResponse> MultiFilters { get; set; } = new();
}