namespace BlazorElectronics.Shared.Products.Search;

public class SearchFiltersVideo
{
    public IntRangeDto? Runtime { get; set; }
    public IntRangeDto? Episodes { get; set; }
    public bool? HasSubtitles { get; set; }
}