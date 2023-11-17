namespace BlazorElectronics.Server.Models.Categories;

public sealed class TertiaryCategoryModel
{
    public short TertiaryCategoryId { get; set; }
    public short SecondaryCategoryId { get; set; }
    public short PrimaryCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}