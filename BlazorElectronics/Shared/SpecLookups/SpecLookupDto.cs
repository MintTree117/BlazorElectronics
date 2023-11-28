namespace BlazorElectronics.Shared.SpecLookups;

public sealed class SpecLookupDto
{
    public int SpecId { get; set; }
    public string SpecName { get; set; } = string.Empty;
    public List<string> Values { get; set; } = new();
}