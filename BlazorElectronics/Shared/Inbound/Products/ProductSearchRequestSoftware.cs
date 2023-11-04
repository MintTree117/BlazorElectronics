namespace BlazorElectronics.Shared.Inbound.Products;

public sealed class ProductSearchRequestSoftware : ProductSearchRequest
{
    public int? SoftwareDeveloperId { get; set; }

    public Dictionary<int, List<int>>? SoftwareDynamicFiltersInclude { get; set; }
    public Dictionary<int, List<int>>? SoftwareDynamicFiltersExclude { get; set; }
}