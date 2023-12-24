namespace BlazorElectronics.Shared.Features;

public sealed class FeatureDealDto
{
    public int ProductId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal SalePrice { get; set; }
    public int Rating { get; set; }
    public int NumberReviews { get; set; }
}