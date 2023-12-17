namespace BlazorElectronics.Shared.Specs;

public sealed class LookupSpec
{
    public int SpecId { get; init; }
    public string SpecName { get; init; } = string.Empty;
    public bool IsAvoid { get; set; }
    public List<string> Values { get; init; } = new();
}