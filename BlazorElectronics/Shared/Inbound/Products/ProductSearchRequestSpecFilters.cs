namespace BlazorElectronics.Shared.Inbound.Products;

public sealed class ProductSearchRequestSpecFilters
{
    public Dictionary<short, short>? ExplicitIntSpecsMinimums { get; set; }
    public Dictionary<short, short>? ExplicitIntSpecsMaximums { get; set; }
    
    public Dictionary<short, string>? ExplicitStringSpecsEqualities { get; set; }
    
    public Dictionary<short, List<short>>? DynamicSpecsIncludeIdsByTable { get; set; }
    public Dictionary<short, List<short>>? DynamicSpecsExcludeIdsByTable { get; set; }
}