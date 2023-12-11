using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Shared.Categories;

public sealed class CategoryView : CrudView
{
    public CategoryTier Tier { get; init; }
    public int? ParentCategoryId { get; set; }
}