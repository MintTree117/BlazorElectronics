namespace BlazorElectronics.Shared.Inbound.Products;

public sealed class ProductSearchRequestBooks : ProductSearchRequest
{
    public int? PublisherId { get; set; }
    public int? AuthorId { get; set; }
    public int? MinPages { get; set; }
    public int? MaxPages { get; set; }
    public bool? HasAccessibility { get; set; }
    public bool? HasAudio { get; set; }
    public int? MinAudioLength { get; set; }
    public int? MaxAudioLength { get; set; }

    public Dictionary<int, List<int>>? BooksDynamicFiltersInclude { get; set; }
    public Dictionary<int, List<int>>? BooksDynamicFiltersExclude { get; set; }
}