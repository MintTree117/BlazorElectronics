namespace BlazorElectronics.Server.Dtos.Specs;

public sealed class SpecDataDto
{
    public HashSet<short> IntGlobalIds { get; } = new();
    public HashSet<short> StringGlobalIds { get; } = new();
    public HashSet<short> BoolGlobalIds { get; } = new();
    
    public Dictionary<short, string> IntNames { get; } = new();
    public Dictionary<short, string> StringNames { get; } = new();
    public Dictionary<short, string> BoolNames { get; } = new();
    
    public Dictionary<short, HashSet<short>> IntIdsByCategory { get; } = new();
    public Dictionary<short, HashSet<short>> StringIdsByCategory { get; } = new();
    public Dictionary<short, HashSet<short>> BoolIdsByCategory { get; } = new();

    public Dictionary<short, List<string>> IntFilters { get; } = new();
    public Dictionary<short, List<string>> StringValues { get; } = new();

    public HashSet<short> MultiGlobalTableIds { get; } = new();
    public Dictionary<short, HashSet<short>> MultiTableCategories { get; } = new();
    public Dictionary<short, string> MultiTableNames { get; } = new();
    public Dictionary<short, string> MultiDisplayNames { get; } = new();
    public Dictionary<short, string> MultiProductTableNames { get; } = new();
    public Dictionary<short, List<string>> MultiValuesByTable { get; } = new();
}