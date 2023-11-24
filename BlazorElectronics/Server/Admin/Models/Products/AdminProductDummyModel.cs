namespace BlazorElectronics.Server.Admin.Models.Products;

public abstract class AdminProductDummyModel
{
    public string Title { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public DateTime ReleaseDate { get; set; }
    public int NumberSold { get; set; }
    public bool HasDrm { get; set; }

    public int PrimaryCategoryId { get; set; }
    public List<int> SecondaryCategoryIds { get; set; } = new();
    public List<int> TertiaryCategoryIds { get; set; } = new();
    
    public int? VariantId { get; set; }
    public int? VariantValueId { get; set; }

    public Dictionary<int, int> SpecLookupsIntValues { get; set; } = new();
    public Dictionary<int, int> SpecLookupsIntFilters { get; set; } = new();
    public Dictionary<int, int> SpecLookupsString { get; set; } = new();
    public Dictionary<int, bool> SpecLookupsBool { get; set; } = new();
    public Dictionary<int, int> SpecLookupsMulti { get; set; } = new();
}