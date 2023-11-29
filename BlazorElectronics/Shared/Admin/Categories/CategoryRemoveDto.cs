using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;

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