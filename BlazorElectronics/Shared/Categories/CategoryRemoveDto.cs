using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Shared.Categories;

public sealed class CategoryRemoveDto
{
    public CategoryRemoveDto()
    {
        
    }
    public CategoryRemoveDto( int categoryId, CategoryTier categoryTier )
    {
        CategoryId = categoryId;
        CategoryTier = categoryTier;
    }
    
    public CategoryTier CategoryTier { get; init; }
    public int CategoryId { get; init; }
}