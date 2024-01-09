namespace BlazorElectronics.Shared.Products;

public class ProductEditDto : ICrudEditDto
{
    public int ProductId { get; set; }
    public int VendorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public DateTime ReleaseDate { get; set; }
    public bool IsFeatured { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<int> Categories { get; set; } = new();
    public Dictionary<int, List<int>> LookupSpecs { get; set; } = new();
    public string ImagesAsString { get; set; } = string.Empty;
    public string XmlSpecsAsString { get; set; } = string.Empty;
    
    public void SetId( int id )
    {
        ProductId = id;
    }
}