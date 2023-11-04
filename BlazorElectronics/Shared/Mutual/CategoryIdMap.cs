namespace BlazorElectronics.Shared.Mutual;

public sealed class CategoryIdMap
{
    public CategoryIdMap( int tier, int categoryId )
    {
        Tier = tier;
        CategoryId = categoryId;
    }
    
    public int Tier { get; set; }
    public int CategoryId { get; set; }
}