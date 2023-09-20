namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecCategoryRaw
{
    public int SpecCategoryRawId { get; set; }
    public int FK_SpecCategoryRaw_CategoryId { get; set; }
    public int FK_SpecCategoryRaw_SpecId { get; set; }
}