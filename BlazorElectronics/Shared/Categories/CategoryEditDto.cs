using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Shared.Categories;

public sealed class CategoryEditDto
{
    public string Name { get; set; } = string.Empty;
    public CategoryType Type { get; set; }
    public int PrimaryCategoryId { get; set; }
    public int? SecondaryCategoryId { get; set; }
    public int? TertiaryCategoryId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}