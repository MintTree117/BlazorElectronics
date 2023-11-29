namespace BlazorElectronics.Shared.Products.Search;

public class SearchFiltersGames
{
    public bool? HasMultiplayer { get; set; }
    public bool? HasOffline { get; set; }
    public bool? HasController { get; set; }
    public bool? HasPurchases { get; set; }
    public IntRangeDto? FileSize { get; set; }
}