namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecCategoryLookup
{
    public int SpecCategoryLookupId { get; set; }
    public int FK_SpecCategoryLookup_CategoryId { get; set; }
    public int FK_SpecCategoryLookup_SpecId { get; set; }
}