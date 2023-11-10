namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecLookupFilterModel
{
    public short SpecId { get; set; }
    public short FilterId { get; set; }
    public object? SpecValue { get; set; }
}