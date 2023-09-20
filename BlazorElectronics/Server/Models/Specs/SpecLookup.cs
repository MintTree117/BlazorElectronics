namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecLookup
{
    public int SpecId { get; set; }
    public int FK_SpecLookup_SpecDataTypeId { get; set; }
    public string? SpecName { get; set; }
}