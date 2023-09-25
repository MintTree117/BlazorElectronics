namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecLookup
{
    public int LookupId { get; set; }
    public int SpecId { get; set; }
    public object? LookupValue { get; set; }
}