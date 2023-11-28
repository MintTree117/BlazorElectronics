using BlazorElectronics.Shared.Categories;

namespace BlazorElectronics.Server.Dtos.Categories;

public sealed class CategoryData
{
    public CategoryIds Ids { get; set; } = new();
    public CategoryUrlMap Urls { get; set; }
    public CategoriesResponse Response { get; set; } = new();
}