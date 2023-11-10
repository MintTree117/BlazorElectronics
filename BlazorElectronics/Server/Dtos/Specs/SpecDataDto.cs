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

    public Dictionary<short, List<string>> IntFilterNames { get; set; } = new();
    public Dictionary<short, List<string>> StringFilterNames { get; set; } = new();

    public HashSet<short> MultiGlobalTableIds { get; set; } = new();
    public Dictionary<short, HashSet<short>> MultiTableCategories { get; set; } = new();
    public Dictionary<short, string> MultiTableNames { get; set; } = new();
    public Dictionary<short, string> MultiDisplayNames { get; set; } = new();
    public Dictionary<short, string> MultiProductTableNames { get; set; } = new();
    public Dictionary<short, List<string>> MultiValuesByTable { get; set; } = new();
}