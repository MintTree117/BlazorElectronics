namespace BlazorElectronics.Shared.Admin.Categories;

public sealed class UpdateCategoryDto
{
    public UpdateCategoryDto( 
        int updateCategoryId, int updateCategoryTier, string? categoryName, int categoryTier, int? primaryCategoryId,
        int? secondaryCategoryId, int? tertiaryCategoryId, string? categoryDescription, string? categoryApiUrl, string? categoryImageUrl )
    {
        UpdateCategoryId = updateCategoryId;
        UpdateCategoryTier = updateCategoryTier;
        CategoryName = categoryName;
        CategoryTier = categoryTier;
        PrimaryCategoryId = primaryCategoryId;
        SecondaryCategoryId = secondaryCategoryId;
        TertiaryCategoryId = tertiaryCategoryId;
        CategoryDescription = categoryDescription;
        CategoryApiUrl = categoryApiUrl;
        CategoryImageUrl = categoryImageUrl;
    }
    
    public int UpdateCategoryId { get; }
    public int UpdateCategoryTier { get; }

    public string? CategoryName { get; }
    public int CategoryTier { get; }
    public int? PrimaryCategoryId { get; }
    public int? SecondaryCategoryId { get; }
    public int? TertiaryCategoryId { get; }
    public string? CategoryDescription { get; }
    public string? CategoryApiUrl { get; }
    public string? CategoryImageUrl { get; }
}