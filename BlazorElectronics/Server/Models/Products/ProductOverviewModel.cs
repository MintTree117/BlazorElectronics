namespace BlazorElectronics.Server.Models.Products;

public sealed class ProductOverviewModel
{
    public int PrimaryCategoryId { get; set; }
    public int SecondaryCategoryId { get; set; }
    public int TertiaryCategoryId { get; set; }
    public Product? Product { get; set; }
    public ProductDescription? ProductDescription;
    public List<ProductImage> ProductImages { get; set; } = new();
}