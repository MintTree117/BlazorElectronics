namespace BlazorElectronics.Server.Models.Categories;

public sealed class SecondaryCategoryModel
{
    public short SecondaryCategoryId { get; set; }
    public short PrimaryCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}