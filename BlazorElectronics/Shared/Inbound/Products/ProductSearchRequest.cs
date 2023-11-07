namespace BlazorElectronics.Shared.Inbound.Products;

public class ProductSearchRequest
{
    public int Page { get; set; } = 1;
    public int Rows { get; set; } = 10;

    public string? SearchText { get; set; } = null;
    
    public bool MustHaveSale { get; set; } = false;
    
    public int? MinPrice { get; set; } = null;
    public int? MaxPrice { get; set; } = null;
    public int? MinRating { get; set; } = null;
    public int? MaxRating { get; set; } = null;

    public ProductSearchRequestSpecFilters? SpecFilters { get; set; }
}