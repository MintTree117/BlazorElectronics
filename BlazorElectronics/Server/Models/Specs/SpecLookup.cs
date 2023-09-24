namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecLookup
{
    public int SpecLookupId { get; set; }
    public int LookupId { get; set; }
    public object? LookupValue { get; set; }
}