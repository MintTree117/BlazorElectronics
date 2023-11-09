namespace BlazorElectronics.Server.Dtos.Specs;

public sealed class CachedSpecData
{
    public CachedSpecData( SpecDataDto dto )
    {
        LastFetched = DateTime.Now;

        IntGlobalIds = new HashSet<short>( dto.IntGlobalIds );
        StringGlobalIds = new HashSet<short>( dto.StringGlobalIds );
        IntNames = new Dictionary<short, string>( dto.IntNames );
        StringNames = new Dictionary<short, string>( dto.StringNames );
        
        var explicitIntIdsByCategorySets = new List<IReadOnlySet<short>>();
        var explicitStringIdsByCategorySets = new List<IReadOnlySet<short>>();

        Dictionary<short, HashSet<short>>.KeyCollection explicitIntCategoryKeys = dto.IntIdsByCategory.Keys;
        Dictionary<short, HashSet<short>>.KeyCollection explicitStringCategoryKeys = dto.StringIdsByCategory.Keys;
        
        foreach ( short category in explicitIntCategoryKeys )
            explicitIntIdsByCategorySets.Add( new HashSet<short>( dto.IntIdsByCategory[ category ] ) );
        
        foreach ( short category in explicitStringCategoryKeys )
            explicitStringIdsByCategorySets.Add( new HashSet<short>( dto.StringIdsByCategory[ category ] ) );

        var explicitIntIdsByCategoryDict = new Dictionary<short, IReadOnlySet<short>>();
        var explicitStringIdsByCategoryDict = new Dictionary<short, IReadOnlySet<short>>();
        
        foreach ( short category in explicitIntCategoryKeys )
            explicitIntIdsByCategoryDict.Add( category, explicitIntIdsByCategorySets[ category ] );

        foreach ( short category in explicitStringCategoryKeys )
            explicitStringIdsByCategoryDict.Add( category, explicitStringIdsByCategorySets[ category ] );

        IntIdsByCategory = explicitIntIdsByCategoryDict;
        StringIdsByCategory = explicitStringIdsByCategoryDict;

        DynamicGlobalTableIds = new HashSet<short>( dto.DynamicGlobalTableIds );
        DynamicTableNames = new Dictionary<short, string>( dto.DynamicTableNames );
        DynamicDisplayNames = new Dictionary<short, string>( dto.DynamicDisplayNames );
        DynamicProductTableNames = new Dictionary<short, string>( dto.DynamicProductTableNames );

        var dynamicTableIdsByCategorySets = new List<IReadOnlySet<short>>();

        Dictionary<short, HashSet<short>>.KeyCollection dynamicCategoryKeys = dto.DynamicTableCategories.Keys;

        foreach ( short category in dynamicCategoryKeys )
            dynamicTableIdsByCategorySets.Add( new HashSet<short>( dto.DynamicTableCategories[ category ] ) );

        var dynamicIdsByCategoryDict = new Dictionary<short, IReadOnlySet<short>>();

        foreach ( short category in dynamicCategoryKeys )
            explicitIntIdsByCategoryDict.Add( category, dynamicTableIdsByCategorySets[ category ] );

        DynamicTablesByCategory = dynamicIdsByCategoryDict;

        /*var dynamicResponseByTableIdDict = new Dictionary<short, IReadOnlyList<DynamicSpecFilterValueResponse>>();

        foreach ( short tableId in dto.DynamicResponses.Keys )
            dynamicResponseByTableIdDict.TryAdd( tableId, dto.DynamicResponses[ tableId ] );

        DynamicResponsesByTable = dynamicResponseByTableIdDict;*/
    }

    public bool IsValid( int maxHours )
    {
        return ( DateTime.Now - LastFetched ).TotalHours < maxHours;
    }

    readonly DateTime LastFetched;

    public IReadOnlySet<short> IntGlobalIds { get; init; }
    public IReadOnlySet<short> StringGlobalIds { get; init; }
    public IReadOnlySet<short> BoolGlobalIds { get; init; }
    
    public IReadOnlyDictionary<short, string> IntNames { get; init; }
    public IReadOnlyDictionary<short, string> StringNames { get; init; }
    public IReadOnlyDictionary<short, string> BoolNames { get; init; }

    public IReadOnlyDictionary<short, IReadOnlySet<short>> IntIdsByCategory { get; init; }
    public IReadOnlyDictionary<short, IReadOnlySet<short>> StringIdsByCategory { get; init; }
    public IReadOnlyDictionary<short, IReadOnlySet<short>> BoolIdsByCategory { get; init; }
    
    public IReadOnlyDictionary<short, IReadOnlyList<string>> IntFilterValues { get; init; }
    public IReadOnlyDictionary<short, IReadOnlyList<string>> StringValues { get; init; }

    public IReadOnlySet<short> DynamicGlobalTableIds { get; init; }
    public IReadOnlyDictionary<short, string> DynamicTableNames { get; init; }
    public IReadOnlyDictionary<short, string> DynamicDisplayNames { get; init; }
    public IReadOnlyDictionary<short, string> DynamicProductTableNames { get; init; }
    public IReadOnlyDictionary<short, IReadOnlySet<short>> DynamicTablesByCategory { get; init; }
    public IReadOnlyDictionary<short, IReadOnlyList<string>> DynamicValuesByTable { get; init; }
}