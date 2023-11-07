namespace BlazorElectronics.Shared.Mutual;

public sealed class CategoryIdMap
{
    public CategoryIdMap( int tier, short categoryId )
    {
        Tier = tier;
        CategoryId = categoryId;
    }
    
    public int Tier { get; set; }
    public short CategoryId { get; set; }
}