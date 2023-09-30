namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecDescr
{
    public int SpecId { get; set; }
    public int DataTypeId { get; set; }
    public bool IsDynamic { get; set; }
    public string? SpecName { get; set; }
    public List<SpecCategory> SpecCategories { get; set; } = new();
}