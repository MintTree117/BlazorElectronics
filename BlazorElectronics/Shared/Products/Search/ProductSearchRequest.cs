namespace BlazorElectronics.Shared.Products.Search;

public class ProductSearchRequest
{
    public int Page { get; set; } = 1;
    public int Rows { get; set; } = 10;

    public ProductSearchSort SortType { get; set; }
    public DateTime? MinReleaseDate { get; set; }
    
    public string? SearchText { get; set; } = null;
    
    public bool? InStock { get; set; } = false;
    public bool? HasSale { get; set; } = false;
    public bool? HasDrm { get; set; }
    
    public int? MinPrice { get; set; } = null;
    public int? MaxPrice { get; set; } = null;
    public int? MinRating { get; set; } = null;

    public List<int>? Vendors { get; set; }
    public Dictionary<int, List<int>>? LookupIncludes { get; set; }
    public Dictionary<int, List<int>>? LookupExcludes { get; set; }
}