namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecValue
{
    public int SpecValueId { get; set; }
    public int SpecId { get; set; }
    public int DataTypeId { get; set; }
    public object? Value { get; set; }
}