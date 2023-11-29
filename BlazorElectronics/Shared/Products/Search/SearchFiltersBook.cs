namespace BlazorElectronics.Shared.Products.Search;

public sealed class SearchFiltersBook
{
    public IntRangeDto? Pages { get; set; }
    public bool? HasAudio { get; set; }
    public IntRangeDto? AudioLength { get; set; }
}