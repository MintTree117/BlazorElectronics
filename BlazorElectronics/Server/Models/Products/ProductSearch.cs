namespace BlazorElectronics.Server.Models.Products;

public sealed class ProductSearch
{
    public IEnumerable<Product>? Products { get; set; }
    public int Count { get; set; }
}