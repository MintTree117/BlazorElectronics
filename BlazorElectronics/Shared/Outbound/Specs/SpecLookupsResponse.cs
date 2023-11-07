namespace BlazorElectronics.Shared.Outbound.Specs;

public sealed class SpecLookupsResponse
{
    public List<SpecLookupTableResponse> Lookups { get; set; } = new();
}