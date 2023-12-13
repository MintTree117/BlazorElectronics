namespace BlazorElectronics.Server.Core.Models.Products;

public sealed class ProductSearchModel
{
    public int TotalCount { get; init; }
    public int ProductId { get; init; }
    public string Title { get; init; } = string.Empty;
    public float Rating { get; init; }
    public string Thumbnail { get; init; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public int NumberSold { get; set; }
    public int NumberReviews { get; set; }
}