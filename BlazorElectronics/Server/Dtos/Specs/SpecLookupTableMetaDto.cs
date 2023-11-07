using BlazorElectronics.Shared.Outbound.Specs;

namespace BlazorElectronics.Server.Dtos.Specs;

public sealed class SpecLookupTableMetaDto
{
    public SpecLookupTableMetaDto()
    {
        LastFetched = DateTime.Now;
    }

    public bool IsValid( int maxHours )
    {
        return ( DateTime.Now - LastFetched ).TotalHours < maxHours;
    }

    readonly DateTime LastFetched;
    
    public HashSet<short> ExplicitIntGlobalIds { get; set; } = new();
    public HashSet<short> ExplicitStringGlobalIds { get; set; } = new();
    public Dictionary<short, HashSet<short>> ExplicitIntIdsByCategory { get; set; } = new();
    public Dictionary<short, HashSet<short>> ExplicitStringIdsByCategory { get; set; } = new();
    public Dictionary<short, string> ExplicitIntNames { get; set; } = new();
    public Dictionary<short, string> ExplicitStringNames { get; set; } = new();
    
    public HashSet<short> DynamicGlobalTableIds { get; set; } = new();
    public Dictionary<short, HashSet<short>> DynamicTableIdsByCategory { get; set; } = new();
    public Dictionary<short, string> DynamicSpecTableNames { get; set; } = new();
    public Dictionary<short, string> DynamicSpecDisplayNames { get; set; } = new();
    public Dictionary<short, string> DynamicProductTableNames { get; set; } = new();
    public Dictionary<short, List<SpecLookupValueResponse>> DynamicResponseByTableId { get; set; } = new();
}