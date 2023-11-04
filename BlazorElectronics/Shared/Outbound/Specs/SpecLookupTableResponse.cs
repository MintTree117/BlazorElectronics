namespace BlazorElectronics.Shared.Outbound.Specs;

public sealed class SpecLookupTableResponse
{
    public int LookupTableId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<SpecLookupResponse> SpecLookups { get; set; } = new();
}