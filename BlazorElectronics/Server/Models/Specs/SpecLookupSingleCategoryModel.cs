namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecLookupSingleCategoryModel
{
    public SpecLookupSingleCategoryModel( short specId, short primaryCategoryId )
    {
        SpecId = specId;
        PrimaryCategoryId = primaryCategoryId;
    }
    
    public short SpecId { get; }
    public short PrimaryCategoryId { get; }
}
