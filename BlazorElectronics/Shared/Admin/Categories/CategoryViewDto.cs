namespace BlazorElectronics.Shared.Admin.Categories;

public sealed class CategoryViewDto
{
    public int PrimaryCategory { get; set; }
    public int? SecondaryCategory { get; set; }
    public int? TertiaryCategory { get; set; }
    public string Name { get; set; } = string.Empty;
}