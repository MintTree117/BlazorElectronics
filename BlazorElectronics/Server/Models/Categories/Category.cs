namespace BlazorElectronics.Server.Models.Categories;

public sealed class Category
{
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? CategoryUrl { get; set; }
    public string? CategoryImageUrl { get; set; }
    public bool IsPrimaryCategory { get; set; } = false;

    public List<CategorySub> SubCategories { get; set; } = new();
}