namespace BlazorElectronics.Shared.Admin.SpecLookups;

public sealed class SpecLookupEditDto
{
    public int SpecId { get; set; }
    public string SpecName { get; set; } = string.Empty;
    public bool IsGlobal { get; set; }
    public string PrimaryCategoriesAsString { get; set; } = string.Empty;
    public string ValuesByIdAsString { get; set; } = string.Empty;
}