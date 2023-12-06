namespace BlazorElectronics.Shared.SpecLookups;

public sealed class SpecLookup
{
    public int SpecId { get; init; }
    public string SpecName { get; init; } = string.Empty;
    public List<string> Values { get; init; } = new();
}