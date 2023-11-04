namespace BlazorElectronics.Shared.Outbound.Specs;

public sealed class SpecsLookupsCategoryResponse : SpecLookupsGlobalResponse
{
    public List<SpecLookupTableResponse> CategoryLookups { get; set; } = new();
}