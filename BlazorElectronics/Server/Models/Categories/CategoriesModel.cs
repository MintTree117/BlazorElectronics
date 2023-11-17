namespace BlazorElectronics.Server.Models.Categories;

public sealed class CategoriesModel
{
    public IEnumerable<PrimaryCategoryModel>? Primary { get; set; }
    public IEnumerable<SecondaryCategoryModel>? Secondary { get; set; }
    public IEnumerable<TertiaryCategoryModel>? Tertiary { get; set; }
}