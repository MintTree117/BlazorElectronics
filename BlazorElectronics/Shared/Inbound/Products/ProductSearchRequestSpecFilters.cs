namespace BlazorElectronics.Shared.Inbound.Products;

public sealed class ProductSearchRequestSpecFilters
{
    // Key = SpecId, Values = FilterIds
    public Dictionary<short, List<short>>? IntFilters { get; set; }
    public Dictionary<short, List<short>>? StringFilters { get; set; }
    
    // Key = SpecId, Value = SpecValue
    public Dictionary<short, bool>? BoolFilters { get; set; }
    
    // Key = MultiTableId, Values = SpecIds
    public Dictionary<short, List<short>>? MultiIncludes { get; set; }
    public Dictionary<short, List<short>>? MultiExcludes { get; set; }
}