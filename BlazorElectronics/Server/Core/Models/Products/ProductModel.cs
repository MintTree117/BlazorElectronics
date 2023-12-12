namespace BlazorElectronics.Server.Core.Models.Products;

public sealed class ProductModel
{
    public int ProductId { get; set; }
    public int VendorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public DateTime ReleaseDate { get; set; }
    public int NumberSold { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<string> Images { get; set; } = new();
    public List<int> Categories { get; set; } = new();
    public Dictionary<int, List<int>> SpecLookups { get; set; } = new();
    public string XmlSpecs { get; set; } = string.Empty;
    public List<ProductReview> Reviews { get; set; } = new();
}