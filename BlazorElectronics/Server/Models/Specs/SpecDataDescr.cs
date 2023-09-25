namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecDataDescr
{
    public int DataTypeId { get; set; }
    public Type? DataType { get; set; }
    public SpecFilterType FilterType { get; set; }
}