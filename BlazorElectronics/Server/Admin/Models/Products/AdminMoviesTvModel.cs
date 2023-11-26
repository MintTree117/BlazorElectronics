namespace BlazorElectronics.Server.Admin.Models.Products;

public sealed class AdminMoviesTvModel : AdminProductModel
{
    public string Director { get; set; } = string.Empty;
    public string Cast { get; set; } = string.Empty;
    public int RuntimeMinutes { get; set; }
    public int Episodes { get; set; }
    public bool HasSubtitles { get; set; }
    public List<int> Awards { get; set; } = new();
}