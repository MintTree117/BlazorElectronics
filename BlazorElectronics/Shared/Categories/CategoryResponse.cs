namespace BlazorElectronics.Shared.Categories;

public sealed class CategoryResponse
{
    public int Id { get; init; }
    public int ParentId { get; set; }
    public string Name { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public List<int> Children { get; set; } = new();
}