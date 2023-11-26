using BlazorElectronics.Server.Admin.Models.Variants;

namespace BlazorElectronics.Server.Admin.Models.Products;

public abstract class AdminProductModel
{
    public string Title { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public float Rating { get; set; }
    public DateTime ReleaseDate { get; set; }
    public int NumberSold { get; set; }
    public bool HasDrm { get; set; }
    public bool HasSale { get; set; }
    
    public int PrimaryCategoryId { get; set; }
    public List<int> SecondaryCategoryIds { get; set; } = new();
    public List<int> TertiaryCategoryIds { get; set; } = new();

    public List<VariantModel> Variants { get; set; } = new();

    public List<int> Languages { get; set; } = new();
    public List<int> MatureContent { get; set; } = new();
}