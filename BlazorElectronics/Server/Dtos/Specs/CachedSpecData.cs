namespace BlazorElectronics.Server.Dtos.Specs;

public sealed class CachedSpecData : LocallyCachedObject
{
    public CachedSpecData( SpecDataDto dto )
    {
        IntGlobalIds = new HashSet<short>( dto.IntGlobalIds );
        StringGlobalIds = new HashSet<short>( dto.StringGlobalIds );
        BoolGlobalIds = new HashSet<short>( dto.BoolGlobalIds );
        MultiGlobalIds = new HashSet<short>( dto.MultiGlobalIds );
        
        IntNames = new Dictionary<short, string>( dto.IntNames );
        StringNames = new Dictionary<short, string>( dto.StringNames );
        BoolNames = new Dictionary<short, string>( dto.BoolNames );
        MultiNames = new Dictionary<short, string>( dto.MultiNames );

        IntIdsByCategory = GetIdsByCategory( dto.IntIdsByCategory );
        StringIdsByCategory = GetIdsByCategory( dto.StringIdsByCategory );
        BoolIdsByCategory = GetIdsByCategory( dto.BoolIdsByCategory );
        MultiIdsByCategory = GetIdsByCategory( dto.MultiIdsByCategory );

        IntFilters = GetValuesById( dto.IntFilters );
        StringValues = GetValuesById( dto.StringValues );
        MultiValues = GetValuesById( dto.MultiValues );
    }

    public IReadOnlySet<short> IntGlobalIds { get; }
    public IReadOnlySet<short> StringGlobalIds { get; }
    public IReadOnlySet<short> BoolGlobalIds { get; }
    public IReadOnlySet<short> MultiGlobalIds { get; }

    public IReadOnlyDictionary<short, IReadOnlySet<short>> IntIdsByCategory { get; }
    public IReadOnlyDictionary<short, IReadOnlySet<short>> StringIdsByCategory { get; }
    public IReadOnlyDictionary<short, IReadOnlySet<short>> BoolIdsByCategory { get; }
    public IReadOnlyDictionary<short, IReadOnlySet<short>> MultiIdsByCategory { get; }
    
    public IReadOnlyDictionary<short, string> IntNames { get; }
    public IReadOnlyDictionary<short, string> StringNames { get; }
    public IReadOnlyDictionary<short, string> BoolNames { get; }
    public IReadOnlyDictionary<short, string> MultiNames { get; }
    
    public IReadOnlyDictionary<short, IReadOnlyList<string>> IntFilters { get; }
    public IReadOnlyDictionary<short, IReadOnlyList<string>> StringValues { get; }
    public IReadOnlyDictionary<short, IReadOnlyList<string>> MultiValues { get; }

    static IReadOnlyDictionary<short, IReadOnlySet<short>> GetIdsByCategory( Dictionary<short, HashSet<short>> dto )
    {
        var tempCategories = new Dictionary<short, IReadOnlySet<short>>();

        foreach ( short category in dto.Keys )
        {
            HashSet<short> tempSet = dto[ category ];
            tempCategories.Add( category, new HashSet<short>( tempSet ) );
        }

        return tempCategories;
    }
    static IReadOnlyDictionary<short, IReadOnlyList<string>> GetValuesById( Dictionary<short, List<string>> dto )
    {
        var tempValues = new Dictionary<short, IReadOnlyList<string>>();
        
        foreach ( short id in dto.Keys )
        {
            List<string> tempList = dto[ id ];
            tempValues.Add( id, new List<string>( tempList ) );
        }

        return tempValues;
    }
}