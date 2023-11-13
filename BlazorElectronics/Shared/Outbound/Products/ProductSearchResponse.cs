namespace BlazorElectronics.Shared.DtosOutbound.Products;

public sealed class ProductSearchResponse
{
    public List<ProductResponse> Products { get; set; } = new();
    public int TotalMatches { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int ItemsPerPage { get; set; }
}