namespace BlazorElectronics.Shared.Products;

public sealed class ProductResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public float Rating { get; set; }
}