using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Shared.Reviews;

public sealed class GetProductReviewsDto
{
    public int ProductId { get; set; }
    public int Rows { get; set; }
    public int Page { get; set; }
    public ReviewSortType SortType { get; set; }
}