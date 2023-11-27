namespace BlazorElectronics.Server.Models.Categories;

public sealed class CategoriesModel
{
    public IEnumerable<CategoryModel>? Primary { get; set; }
    public IEnumerable<CategoryModel>? Secondary { get; set; }
    public IEnumerable<CategoryModel>? Tertiary { get; set; }
}