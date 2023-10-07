using BlazorElectronics.Shared.DataTransferObjects.Categories;

namespace BlazorElectronics.Server.Models.Categories;

public sealed class CategoryMeta
{
    public Dictionary<int, Category_DTO> CategoriesById { get; set; } = new();
    public Dictionary<string, int> CategoryIdsByUrl { get; set; } = new();
    public List<int> PrimaryCategoryIds { get; set; } = new();
}