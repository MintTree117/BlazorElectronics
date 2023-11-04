namespace BlazorElectronics.Shared.Outbound.Specs;

public sealed class SpecLookupResponse
{
    public int SpecLookupId { get; set; }
    public string SpecLookupName { get; set; } = string.Empty;
    public string SpecLookupValue { get; set; } = string.Empty;
}