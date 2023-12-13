namespace BlazorElectronics.Shared.Products.Search;

public sealed class ProductSearchResponse
{
    public int TotalMatches { get; set; }
    public List<ProductResponse> Products { get; set; } = new();
}