namespace BlazorElectronics.Server.Admin.Models.Products;

public sealed class AdminBookModel : AdminProductModel
{
    public string Author { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string? Narrator { get; set; } = null;
    public string ISBN { get; set; } = string.Empty;
    public int Pages { get; set; }
    public bool HasAudio { get; set; }
    public int AudioLength { get; set; }
    
    public List<int> EbookFormats { get; set; } = new();
    public List<int> AudioBookFormats { get; set; } = new();
}