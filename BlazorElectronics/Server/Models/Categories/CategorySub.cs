namespace BlazorElectronics.Server.Models.Categories;

public sealed class CategorySub
{
    public int CategorySubId { get; set; }
    public int CategoryId { get; set; }
    public int PrimaryCategoryId { get; set; }
}