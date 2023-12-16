namespace BlazorElectronics.Shared.ProductReviews;

public sealed class GetProductReviewsDto
{
    public int ProductId { get; set; }
    public int Rows { get; set; }
    public int Page { get; set; }
}