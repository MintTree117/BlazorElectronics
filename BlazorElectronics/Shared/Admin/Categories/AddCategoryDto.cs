namespace BlazorElectronics.Shared.Admin.Categories;

public sealed class AddCategoryDto
{
    public AddCategoryDto( string? categoryName, int categoryTier, int? primaryCategoryId, int? secondaryCategoryId, int? tertiaryCategoryId, string? categoryDescription, string? categoryApiUrl, string? categoryImageUrl )
    {
        CategoryName = categoryName;
        CategoryTier = categoryTier;
        PrimaryCategoryId = primaryCategoryId;
        SecondaryCategoryId = secondaryCategoryId;
        TertiaryCategoryId = tertiaryCategoryId;
        CategoryDescription = categoryDescription;
        CategoryApiUrl = categoryApiUrl;
        CategoryImageUrl = categoryImageUrl;
    }
    
    public string? CategoryName { get; }
    public int CategoryTier { get; }
    public int? PrimaryCategoryId { get; }
    public int? SecondaryCategoryId { get; }
    public int? TertiaryCategoryId { get; }
    public string? CategoryDescription { get; }
    public string? CategoryApiUrl { get; }
    public string? CategoryImageUrl { get; }
}