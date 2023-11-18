namespace BlazorElectronics.Shared.Admin.Categories;

public sealed class AddUpdateCategoryDto
{
    public int? UpdateCategoryId { get; init; }
    public int? UpdateCategoryTier { get; init; }

    public string? CategoryName { get; init; }
    public int CategoryTier { get; init; }
    public int? PrimaryCategoryId { get; init; }
    public int? SecondaryCategoryId { get; init; }
    public int? TertiaryCategoryId { get; init; }
    public string? CategoryDescription { get; init; }
    public string? CategoryApiUrl { get; init; }
    public string? CategoryImageUrl { get; init; }
}