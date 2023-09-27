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
    public List<int> SpecCategories { get; set; } = new();
    public List<int> SpecFilters { get; set; } = new();
}