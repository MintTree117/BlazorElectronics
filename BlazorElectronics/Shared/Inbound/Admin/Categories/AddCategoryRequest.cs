namespace BlazorElectronics.Shared.Inbound.Admin.Categories;

public class AddCategoryRequest : AdminRequest
{
    public string? CategoryName { get; set; }
    public int CategoryTier { get; set; }
    public int? SecondaryParentId { get; set; }
    public int? TertiaryParentId { get; set; }
    public string? Type { get; set; }
    public string? CategoryDescription { get; set; }
    public string? CategoryApiUrl { get; set; }
    public string? CategoryImageUrl { get; set; }
}