namespace BlazorElectronics.Server.Models.Specs;

public sealed class SpecLookupMultiTableCategoryModel
{
    public SpecLookupMultiTableCategoryModel( short tableId, short primaryCategoryId )
    {
        TableId = tableId;
        PrimaryCategoryId = primaryCategoryId;
    }
    
    public short TableId { get; }
    public short PrimaryCategoryId { get; }
}