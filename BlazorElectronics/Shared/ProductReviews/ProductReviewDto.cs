namespace BlazorElectronics.Shared.ProductReviews;

public sealed class ProductReviewDto
{
    public string Username { get; set; } = string.Empty;
    public string Review { get; set; } = string.Empty;
    public int Rating { get; set; }
}