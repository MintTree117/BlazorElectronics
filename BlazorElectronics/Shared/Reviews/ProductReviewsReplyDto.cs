namespace BlazorElectronics.Shared.Reviews;

public sealed class ProductReviewsReplyDto
{
    public int TotalMatches { get; set; }
    public List<ProductReviewDto> Reviews { get; set; } = new();
}