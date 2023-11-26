namespace BlazorElectronics.Shared.Admin.Categories;

public sealed class CategoriesViewDto
{
    public List<CategoryViewDto> Primary { get; set; } = new();
    public List<CategoryViewDto> Secondary { get; set; } = new();
    public List<CategoryViewDto> Tertiary { get; set; } = new();
}