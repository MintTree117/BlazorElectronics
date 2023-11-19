namespace BlazorElectronics.Shared.Admin.Categories;

public sealed class DeleteCategoryDto
{
    public DeleteCategoryDto()
    {
        
    }
    public DeleteCategoryDto( int categoryId, int categoryTier )
    {
        CategoryId = categoryId;
        CategoryTier = categoryTier;
    }
    
    public int CategoryTier { get; init; }
    public int CategoryId { get; init; }
}