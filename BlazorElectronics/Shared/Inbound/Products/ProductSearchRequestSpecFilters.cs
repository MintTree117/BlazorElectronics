namespace BlazorElectronics.Shared.Inbound.Products;

public sealed class ProductSearchRequestSpecFilters
{
    public Dictionary<short, List<short>>? IntFilterIds { get; set; }
    public Dictionary<short, List<short>>? StringSpecIds { get; set; }
    public Dictionary<short, bool>? BoolSpecIds { get; set; }
    
    public Dictionary<short, List<short>>? DynamicSpecsIncludeIdsByTable { get; set; }
    public Dictionary<short, List<short>>? DynamicSpecsExcludeIdsByTable { get; set; }
}