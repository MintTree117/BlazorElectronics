namespace BlazorElectronics.Shared.Admin.Categories;

public sealed class AddCategoryDto
{
    public AddCategoryDto()
    {
        
    }
    public AddCategoryDto( EditCategoryDto dto )
    {
        Name = dto.Name;
        Tier = dto.Tier;
        PrimaryCategoryId = dto.PrimaryCategoryId;
        SecondaryCategoryId = dto.SecondaryCategoryId;
        TertiaryCategoryId = dto.TertiaryCategoryId;
        Description = dto.Description;
        ApiUrl = dto.ApiUrl;
        ImageUrl = dto.ImageUrl;
    }
    
    public string? Name { get; set; }
    public int Tier { get; set; }
    public int? PrimaryCategoryId { get; set; }
    public int? SecondaryCategoryId { get; set; }
    public int? TertiaryCategoryId { get; set; }
    public string? Description { get; set; }
    public string? ApiUrl { get; set; }
    public string? ImageUrl { get; set; }
}