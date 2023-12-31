namespace BlazorElectronics.Shared.Products.Search;

public sealed class ProductSummaryDto
{
    public int Id { get; set; }
    public int VendorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public float Rating { get; set; }
    public int NumberSold { get; set; }
    public int NumberReviews { get; set; }
    public List<int> Categories { get; set; } = new();
}