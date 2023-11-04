namespace BlazorElectronics.Server.Models.Categories;

public sealed class CategoriesModel
{
    public Dictionary<int, PrimaryCategory> Primary { get; set; } = new();
    public Dictionary<int, SecondaryCategory> Secondary { get; set; } = new();
    public Dictionary<int, TertiaryCategory> Tertiary { get; set; } = new();
}