namespace BlazorElectronics.Shared.Admin.Specs.SpecsSingle;

public sealed class EditSpecLookupDto
{
    public SpecLookupType SpecType { get; set; }
    public string SpecName { get; set; } = string.Empty;
    public bool IsGlobal { get; set; }
    public string ValuesByIdAsString { get; set; } = string.Empty;
    public string PrimaryCategoriesAsString { get; set; } = string.Empty;
}