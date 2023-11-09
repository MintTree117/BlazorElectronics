namespace BlazorElectronics.Server.Models.Specs;

public sealed class IntFilterModel
{
    public short SpecId { get; set; }
    public short FilterId { get; set; }
    public short FilterIndex { get; set; }
    public string FilterValue { get; set; } = string.Empty;
}