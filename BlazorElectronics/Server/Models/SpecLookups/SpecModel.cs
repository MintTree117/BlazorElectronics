namespace BlazorElectronics.Server.Models.SpecLookups;

public sealed class SpecModel
{
    public bool IsGlobal { get; set; }
    public bool IsAvoid { get; set; }
    public int SpecId { get; set; }
    public string SpecName { get; set; } = string.Empty;
}