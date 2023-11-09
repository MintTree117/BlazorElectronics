namespace BlazorElectronics.Shared.Mutual;

public sealed class CategoryIdMap
{
    public CategoryIdMap( int categoryTier, short categoryId )
    {
        CategoryTier = categoryTier;
        CategoryId = categoryId;
    }
    
    public int CategoryTier { get; set; }
    public short CategoryId { get; set; }
}