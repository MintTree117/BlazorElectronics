namespace BlazorElectronics.Server.Models.Specs;

public enum FilterType { }

public sealed class SpecDataType
{
    public int DataTypeId { get; set; }
    public Type? DataType { get; set; }
    public FilterType FilterType { get; set; }
}