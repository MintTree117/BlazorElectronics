namespace BlazorElectronics.Server.Models.Products;

public sealed class ProductSearch
{
    public IEnumerable<Product>? Products { get; set; }
    public int TotalSearchCount { get; set; }
    public int QueryRows { get; set; }
    public int QueryOffset { get; set; }
}