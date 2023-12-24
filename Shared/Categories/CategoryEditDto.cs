using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Shared.Categories;

public sealed class CategoryEditDto : ICrudEditDto
{
    public int PrimaryId { get; set; }
    public int CategoryId { get; set; }
    public int? ParentCategoryId { get; set; }
    public CategoryTier Tier { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    
    public void SetId( int id )
    {
        CategoryId = id;
    }
}