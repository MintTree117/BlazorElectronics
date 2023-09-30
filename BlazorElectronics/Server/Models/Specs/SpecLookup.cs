namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecLookup
{
    public int LookupId { get; set; }
    public int SpecId { get; set; }
    public int ValueId { get; set; }
    public object? Value { get; set; }
}