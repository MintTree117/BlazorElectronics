using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Shared.Reviews;

public sealed class ProductReviewsGetDto
{
    public int TotalMatches { get; set; }
    public int ProductId { get; set; }
    public int Rows { get; set; } = 5;
    public int Page { get; set; } = 1;
    public ReviewSortType SortType { get; set; } = ReviewSortType.Date;
}