namespace BlazorElectronics.Server.Models.Specs;

public enum SpecType
{
    Lookup,
    Raw
}

public sealed class Spec
{
    public int SpecId { get; set; }
    public int SpecDataId { get; set; }
    public SpecType SpecType { get; set; }
    public string? SpecName { get; set; }
    public List<SpecCategory> SpecCategories { get; set; } = new();
    public List<SpecFilter> SpecFilters { get; set; } = new();
}