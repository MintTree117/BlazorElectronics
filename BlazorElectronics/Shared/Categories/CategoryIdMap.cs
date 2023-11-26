namespace BlazorElectronics.Shared.Categories;

public sealed class CategoryIdMap
{
    public CategoryIdMap( CategoryType categoryType, int categoryId )
    {
        CategoryType = categoryType;
        CategoryId = categoryId;
    }
    
    public CategoryType CategoryType { get; set; }
    public int CategoryId { get; set; }
}