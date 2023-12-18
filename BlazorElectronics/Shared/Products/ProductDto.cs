using BlazorElectronics.Shared.Cart;

namespace BlazorElectronics.Shared.Products;

public sealed class ProductDto
{
    public int Id { get; set; }
    public int VendorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public DateTime ReleaseDate { get; set; }
    public bool IsFeatured { get; set; }
    public float Rating { get; set; }
    public int NumberReviews { get; set; }
    public int NumberSold { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<int> Categories { get; set; } = new();
    public List<string> Images { get; set; } = new();
    public Dictionary<int, List<int>> LookupSpecs { get; set; } = new();
    public Dictionary<string, string> XmlSpecsAggregated { get; set; } = new();
    
    void TestFunction()
    {
        CartItemDto c = new();
        c.IncreaseQuantity( 5 );
    }
    
}