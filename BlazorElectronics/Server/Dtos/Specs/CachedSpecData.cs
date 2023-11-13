namespace BlazorElectronics.Server.Dtos.Specs;

public sealed class CachedSpecData : LocallyCachedObject
{
    public CachedSpecData( SpecDataDto dto )
    {
        // SINGLE BASIC
        IntGlobalIds = new HashSet<short>( dto.IntGlobalIds );
        StringGlobalIds = new HashSet<short>( dto.StringGlobalIds );
        BoolGlobalIds = new HashSet<short>( dto.BoolGlobalIds );
        IntNames = new Dictionary<short, string>( dto.IntNames );
        StringNames = new Dictionary<short, string>( dto.StringNames );
        BoolNames = new Dictionary<short, string>( dto.BoolNames );
        
        // SINGLE SPEC CATEGORIES
        var tempIntCategories = new Dictionary<short, IReadOnlySet<short>>();
        var tempStringCategories = new Dictionary<short, IReadOnlySet<short>>();
        var tempBoolCategories = new Dictionary<short, IReadOnlySet<short>>();
        foreach ( short category in dto.IntIdsByCategory.Keys )
        {
            HashSet<short> tempSet = dto.IntIdsByCategory[ category ];
            tempIntCategories.Add( category, new HashSet<short>( tempSet ) );
        }
        foreach ( short category in dto.StringIdsByCategory.Keys )
        {
            HashSet<short> tempSet = dto.StringIdsByCategory[ category ];
            tempStringCategories.Add( category, new HashSet<short>( tempSet ) );
        }
        foreach ( short category in dto.BoolIdsByCategory.Keys )
        {
            HashSet<short> tempSet = dto.BoolIdsByCategory[ category ];
            tempBoolCategories.Add( category, new HashSet<short>( tempSet ) );
        }
        IntIdsByCategory = tempIntCategories;
        StringIdsByCategory = tempStringCategories;
        BoolIdsByCategory = tempBoolCategories;
        
        // SINGLE SPEC FILTERS
        var tempIntFilters = new Dictionary<short, IReadOnlyList<string>>();
        var tempStringValues = new Dictionary<short, IReadOnlyList<string>>();
        foreach ( short id in dto.IntFilters.Keys )
        {
            List<string> tempList = dto.IntFilters[ id ];
            tempIntFilters.Add( id, new List<string>( tempList ) );
        }
        foreach ( short id in dto.StringValues.Keys )
        {
            List<string> tempList = dto.StringValues[ id ];
            tempStringValues.Add( id, new List<string>( tempList ) );
        }
        IntFilters = tempIntFilters;
        StringValues = tempStringValues;

        // MULTI BASIC
        MultiGlobalTableIds = new HashSet<short>( dto.MultiGlobalTableIds );
        MultiTableNames = new Dictionary<short, string>( dto.MultiTableNames );
        MultiDisplayNames = new Dictionary<short, string>( dto.MultiDisplayNames );
        MultiProductTableNames = new Dictionary<short, string>( dto.MultiProductTableNames );
        
        // MULTI CATEGORIES
        var tempMultiCategories = new Dictionary<short, IReadOnlySet<short>>();
        foreach ( short category in dto.MultiTableCategories.Keys )
        {
            HashSet<short> tempSet = dto.MultiTableCategories[ category ];
            tempMultiCategories.Add( category, new HashSet<short>( tempSet ) );
        }
        MultiTablesByCategory = tempMultiCategories;

        // MULTI VALUES
        var tempMultiValues = new Dictionary<short, IReadOnlyList<string>>();
        foreach ( short id in dto.MultiValuesByTable.Keys )
        {
            List<string> tempList = dto.MultiValuesByTable[ id ];
            tempMultiValues.Add( id, new List<string>( tempList ) );
        }
        MultiValuesByTable = tempMultiValues;
    }

    public IReadOnlySet<short> IntGlobalIds { get; }
    public IReadOnlySet<short> StringGlobalIds { get; }
    public IReadOnlySet<short> BoolGlobalIds { get; }
    
    public IReadOnlyDictionary<short, string> IntNames { get; }
    public IReadOnlyDictionary<short, string> StringNames { get; }
    public IReadOnlyDictionary<short, string> BoolNames { get; }

    public IReadOnlyDictionary<short, IReadOnlySet<short>> IntIdsByCategory { get; }
    public IReadOnlyDictionary<short, IReadOnlySet<short>> StringIdsByCategory { get; }
    public IReadOnlyDictionary<short, IReadOnlySet<short>> BoolIdsByCategory { get; }
    
    public IReadOnlyDictionary<short, IReadOnlyList<string>> IntFilters { get; }
    public IReadOnlyDictionary<short, IReadOnlyList<string>> StringValues { get; }

    public IReadOnlySet<short> MultiGlobalTableIds { get; }
    public IReadOnlyDictionary<short, string> MultiTableNames { get; }
    public IReadOnlyDictionary<short, string> MultiDisplayNames { get; }
    public IReadOnlyDictionary<short, string> MultiProductTableNames { get; }
    public IReadOnlyDictionary<short, IReadOnlySet<short>> MultiTablesByCategory { get; }
    public IReadOnlyDictionary<short, IReadOnlyList<string>> MultiValuesByTable { get; }
}