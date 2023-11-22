namespace BlazorElectronics.Shared.Admin.Categories;

public sealed class RemoveCategoryDto
{
    public RemoveCategoryDto()
    {
        
    }
    public RemoveCategoryDto( int categoryId, CategoryType categoryType )
    {
        CategoryId = categoryId;
        CategoryType = categoryType;
    }
    
    public CategoryType CategoryType { get; init; }
    public int CategoryId { get; init; }
}