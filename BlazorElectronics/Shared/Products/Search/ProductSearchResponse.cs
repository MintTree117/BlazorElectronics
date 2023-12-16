namespace BlazorElectronics.Shared.Products.Search;

public sealed class ProductSearchResponse
{
    public int TotalMatches { get; set; }
    public List<ProductSummaryResponse> Products { get; set; } = new();
}