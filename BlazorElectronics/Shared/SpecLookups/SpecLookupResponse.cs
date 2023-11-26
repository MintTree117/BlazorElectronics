namespace BlazorElectronics.Shared.SpecLookups;

public sealed class SpecLookupResponse
{
    public int SpecId { get; set; }
    public string SpecName { get; set; } = string.Empty;
    public List<string> Values { get; set; } = new();
}