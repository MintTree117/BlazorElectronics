namespace BlazorElectronics.Shared.Products.Search;

public sealed class ProductSearchReplyDto
{
    public int TotalMatches { get; set; }
    public List<ProductSummaryDto> Products { get; set; } = new();
}