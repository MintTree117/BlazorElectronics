namespace BlazorElectronics.Shared.Mutual;

public sealed class CategoryIdMap
{
    public CategoryIdMap( CategoryType categoryType, short categoryId )
    {
        CategoryType = categoryType;
        CategoryId = categoryId;
    }
    
    public CategoryType CategoryType { get; set; }
    public short CategoryId { get; set; }
}