using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Shared.Admin.Categories;

public sealed class CategoryRemoveDto
{
    public CategoryRemoveDto()
    {
        
    }
    public CategoryRemoveDto( int categoryId, CategoryType categoryType )
    {
        CategoryId = categoryId;
        CategoryType = categoryType;
    }
    
    public CategoryType CategoryType { get; init; }
    public int CategoryId { get; init; }
}