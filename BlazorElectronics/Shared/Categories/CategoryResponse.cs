namespace BlazorElectronics.Shared.Categories;

public class CategoryResponse
{
    public short Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
}