namespace BlazorElectronics.Server.Models.Products.Seed;

public abstract class ProductSeedModel
{
    public int VendorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public DateTime ReleaseDate { get; set; }
    public int NumberSold { get; set; }
    public bool HasDrm { get; set; }

    public string Description { get; set; } = string.Empty;
    
    public int PrimaryCategoryId { get; set; }
    public List<int> SecondaryCategoryIds { get; set; } = new();
    public List<int> TertiaryCategoryIds { get; set; } = new();
    
    public List<int> Languages { get; set; } = new();
    public List<int> MatureContent { get; set; } = new();
}