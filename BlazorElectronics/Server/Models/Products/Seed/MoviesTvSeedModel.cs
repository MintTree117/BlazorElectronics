namespace BlazorElectronics.Server.Models.Products.Seed;

public sealed class MoviesTvSeedModel : ProductSeedModel
{
    public string Director { get; set; } = string.Empty;
    public string Cast { get; set; } = string.Empty;
    public int RuntimeMinutes { get; set; }
    public int Episodes { get; set; }
    public bool HasSubtitles { get; set; }
    public List<int> VideoFormats { get; set; } = new();
    public List<int> Accessibility { get; set; } = new();
    public List<int> Awards { get; set; } = new();
}