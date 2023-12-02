using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Shared.Categories;

public sealed class CategoryViewDto : AdminItemViewDto
{
    public CategoryTier Tier { get; set; }
}