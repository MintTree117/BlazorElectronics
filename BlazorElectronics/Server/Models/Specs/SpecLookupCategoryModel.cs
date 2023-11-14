namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecLookupCategoryModel
{
    public SpecLookupCategoryModel( short specId, short primaryCategoryId )
    {
        SpecId = specId;
        PrimaryCategoryId = primaryCategoryId;
    }
    
    public short SpecId { get; }
    public short PrimaryCategoryId { get; }
}
