namespace BlazorElectronics.Server.Models.Categories;

public sealed class CategoriesModel
{
    public IEnumerable<PrimaryCategory>? Primary { get; set; }
    public IEnumerable<SecondaryCategory>? Secondary { get; set; }
    public IEnumerable<TertiaryCategory>? Tertiary { get; set; }
}