namespace BlazorElectronics.Shared.DataTransferObjects.Categories;

public sealed class Categories_DTO
{
    public Dictionary<int, Category_DTO> CategoriesById { get; set; } = new();
    public List<int> PrimaryCategoryIds { get; set; } = new();
}