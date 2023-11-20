namespace BlazorElectronics.Shared.Admin.Categories;

public sealed class RemoveCategoryDto
{
    public RemoveCategoryDto()
    {
        
    }
    public RemoveCategoryDto( int categoryId, int categoryTier )
    {
        CategoryId = categoryId;
        CategoryTier = categoryTier;
    }
    
    public int CategoryTier { get; init; }
    public int CategoryId { get; init; }
}