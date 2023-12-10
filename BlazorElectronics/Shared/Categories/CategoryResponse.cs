using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Shared.Categories;

public sealed class CategoryResponse
{
    public int Id { get; set; }
    public int? ParentId { get; set; }
    public CategoryTier Tier { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}