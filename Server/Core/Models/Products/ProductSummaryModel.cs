namespace BlazorElectronics.Server.Core.Models.Products;

public sealed class ProductSummaryModel
{
    public int ProductId { get; set; }
    public int VendorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public DateTime ReleaseDate { get; set; }
    public bool IsFeatured { get; set; }
    public float Rating { get; set; }
    public int NumberReviews { get; set; }
    public int NumberSold { get; set; }
}