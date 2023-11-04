namespace BlazorElectronics.Shared.Inbound.Products;

public sealed class ProductSearchRequestMoviesTv : ProductSearchRequest
{
    public bool? IsTvShow { get; set; }
    public bool? IsMovie { get; set; }
    public int? MinRuntime { get; set; }
    public int? MaxRuntime { get; set; }
    public int? MinEpisodes { get; set; }
    public int? MaxEpisodes { get; set; }

    public List<int>? IncludeSubtitleLanguageIds { get; set; }
    public List<int>? IncludeAwardIds { get; set; }
    public List<int>? IncludeMediaFormatIds { get; set; }
    public List<int>? ExcludeMediaFormatIds { get; set; }

    public Dictionary<int, List<int>>? MoviesTvDynamicFiltersInclude { get; set; }
    public Dictionary<int, List<int>>? MoviesTvDynamicFiltersExclude { get; set; }
}