namespace BlazorElectronics.Server.Models.Categories;

public sealed class TertiaryCategory
{
    public int TertiaryCategoryId { get; set; }
    public int SecondaryCategoryId { get; set; }
    public int PrimaryCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}