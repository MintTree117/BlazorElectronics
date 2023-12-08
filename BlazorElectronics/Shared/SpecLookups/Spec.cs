namespace BlazorElectronics.Shared.SpecLookups;

public sealed class Spec
{
    public int SpecId { get; init; }
    public string SpecName { get; init; } = string.Empty;
    public bool IsAvoid { get; set; }
    public List<string> Values { get; init; } = new();
}