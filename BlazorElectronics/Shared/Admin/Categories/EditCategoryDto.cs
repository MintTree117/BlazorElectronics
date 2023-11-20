namespace BlazorElectronics.Shared.Admin.Categories;

public sealed class EditCategoryDto
{
    public int? NewPrimaryCategory { get; set; }
    public int? NewSecondaryCategory { get; set; }
    public int? NewTertiaryCategory { get; set; }
    
    public string? Name { get; set; }
    public int Tier { get; set; }
    public int? PrimaryCategoryId { get; set; }
    public int? SecondaryCategoryId { get; set; }
    public int? TertiaryCategoryId { get; set; }
    public string? Description { get; set; }
    public string? ApiUrl { get; set; }
    public string? ImageUrl { get; set; }
}