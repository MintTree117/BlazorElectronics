namespace BlazorElectronics.Server.Dtos.Specs;

public sealed class SpecDataDto
{
    public HashSet<short> IntGlobalIds { get; set; } = new();
    public HashSet<short> StringGlobalIds { get; set; } = new();
    public HashSet<short> BoolGlobalIds { get; set; } = new();
    
    public Dictionary<short, string> IntNames { get; set; } = new();
    public Dictionary<short, string> StringNames { get; set; } = new();
    public Dictionary<short, string> BoolNames { get; set; } = new();
    
    public Dictionary<short, HashSet<short>> IntIdsByCategory { get; set; } = new();
    public Dictionary<short, HashSet<short>> StringIdsByCategory { get; set; } = new();
    public Dictionary<short, HashSet<short>> BoolIdsByCategory { get; set; } = new();
    
    public Dictionary<short, List<string>> IntFilterValues { get; set; }
    public Dictionary<short, List<string>> StringValues { get; set; }

    public HashSet<short> DynamicGlobalTableIds { get; set; } = new();
    public Dictionary<short, HashSet<short>> DynamicTableCategories { get; set; } = new();
    public Dictionary<short, string> DynamicTableNames { get; set; } = new();
    public Dictionary<short, string> DynamicDisplayNames { get; set; } = new();
    public Dictionary<short, string> DynamicProductTableNames { get; set; } = new();
    public Dictionary<short, List<string>> DynamicValuesByTable { get; set; } = new();
}