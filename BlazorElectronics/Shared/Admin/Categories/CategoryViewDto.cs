namespace BlazorElectronics.Shared.Admin.Categories;

public sealed class CategoryViewDto
{
    public List<CategoryPrimaryViewDto> Primary { get; set; } = new();
    public List<CategorySecondaryViewDto> Secondary { get; set; } = new();
    public List<CategoryTertiaryViewDto> Tertiary { get; set; } = new();
}