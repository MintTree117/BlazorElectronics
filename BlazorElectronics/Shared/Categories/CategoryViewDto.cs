namespace BlazorElectronics.Shared.Categories;

public sealed class CategoryViewDto
{
    public int PrimaryCategoryId { get; set; }
    public int SecondaryCategoryId { get; set; }
    public int TertiaryCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
}