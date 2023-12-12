namespace BlazorElectronics.Shared.Products.Search;

public sealed class ProductSearchFilters
{
    public DateTime? MinReleaseDate { get; set; }
    public bool? InStock { get; set; } = false;
    public bool? OnSale { get; set; } = false;
    public int? MinPrice { get; set; } = null;
    public int? MaxPrice { get; set; } = null;
    public int? MinRating { get; set; } = null;
    
    public List<int> Vendors { get; set; } = new();
    public Dictionary<int, List<int>> SpecsInclude { get; set; } = new();
    public Dictionary<int, List<int>> SpecsExlude { get; set; } = new();
}