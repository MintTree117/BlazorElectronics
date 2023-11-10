namespace BlazorElectronics.Server.Dtos.Specs;

public sealed class CachedSpecData
{
    public CachedSpecData( SpecDataDto dto )
    {
        LastFetched = DateTime.Now;
        
        // SINGLE BASIC
        IntGlobalIds = new HashSet<short>( dto.IntGlobalIds );
        StringGlobalIds = new HashSet<short>( dto.StringGlobalIds );
        BoolGlobalIds = new HashSet<short>( dto.BoolGlobalIds );
        IntNames = new Dictionary<short, string>( dto.IntNames );
        StringNames = new Dictionary<short, string>( dto.StringNames );
        BoolNames = new Dictionary<short, string>( dto.BoolNames );
        
        // SINGLE SPEC CATEGORIES
        Dictionary<short, HashSet<short>>.KeyCollection intCategoryKeys = dto.IntIdsByCategory.Keys;
        Dictionary<short, HashSet<short>>.KeyCollection stringCategoryKeys = dto.StringIdsByCategory.Keys;
        Dictionary<short, HashSet<short>>.KeyCollection boolCategoryKeys = dto.BoolIdsByCategory.Keys;
        var intIdsByCategorySets = new List<IReadOnlySet<short>>();
        var stringIdsByCategorySets = new List<IReadOnlySet<short>>();
        var boolIdsByCategorySets = new List<IReadOnlySet<short>>();
        var intIdsByCategoryDict = new Dictionary<short, IReadOnlySet<short>>();
        var stringIdsByCategoryDict = new Dictionary<short, IReadOnlySet<short>>();
        var boolIdsByCategoryDict = new Dictionary<short, IReadOnlySet<short>>();
        foreach ( short category in intCategoryKeys )
            intIdsByCategorySets.Add( new HashSet<short>( dto.IntIdsByCategory[ category ] ) );
        foreach ( short category in stringCategoryKeys )
            stringIdsByCategorySets.Add( new HashSet<short>( dto.StringIdsByCategory[ category ] ) );
        foreach ( short category in boolCategoryKeys )
            boolIdsByCategorySets.Add( new HashSet<short>( dto.BoolIdsByCategory[ category ] ) );
        int intCatCount = 0;
        int stringCatCount = 0;
        int boolCatCount = 0;
        foreach ( short category in intCategoryKeys )
        {
            intIdsByCategoryDict.Add( category, intIdsByCategorySets[ intCatCount ] );
            intCatCount++;
        }
        foreach ( short category in stringCategoryKeys )
        {
            stringIdsByCategoryDict.Add( category, stringIdsByCategorySets[ stringCatCount ] );
            stringCatCount++;
        }
        foreach ( short category in boolCategoryKeys )
        {
            boolIdsByCategoryDict.Add( category, boolIdsByCategorySets[ boolCatCount ] );
            boolCatCount++;
        }
        IntIdsByCategory = intIdsByCategoryDict;
        StringIdsByCategory = stringIdsByCategoryDict;
        BoolIdsByCategory = boolIdsByCategoryDict;

        // SINGLE SPEC FILTERS
        Dictionary<short, List<string>>.KeyCollection intFilterKeys = dto.IntFilterNames.Keys;
        Dictionary<short, List<string>>.KeyCollection stringFilterKeys = dto.StringFilterNames.Keys;
        var intFilterLists = new List<IReadOnlyList<string>>();
        var stringFilterLists = new List<IReadOnlyList<string>>();
        var intFiltersDict = new Dictionary<short, IReadOnlyList<string>>();
        var stringFiltersDict = new Dictionary<short, IReadOnlyList<string>>();
        foreach ( short filter in intFilterKeys )
            intFilterLists.Add( new List<string>( dto.IntFilterNames[ filter ] ) );
        foreach ( short filter in stringFilterKeys )
            stringFilterLists.Add( new List<string>( dto.StringFilterNames[ filter ] ) );
        int intFilterCount = 0;
        int stringFilterCount = 0;
        foreach ( short filter in intFilterKeys )
        {
            intFiltersDict.Add( filter, intFilterLists[ intFilterCount ] );
            intFilterCount++;
        }
        foreach ( short filter in stringFilterKeys )
        {
            stringFiltersDict.Add( filter, stringFilterLists[ stringFilterCount ] );
            stringFilterCount++;
        }
        IntFilterNames = intFiltersDict;
        StringFilterNames = stringFiltersDict;
        
        // MULTI BASIC
        MultiGlobalTableIds = new HashSet<short>( dto.MultiGlobalTableIds );
        MultiTableNames = new Dictionary<short, string>( dto.MultiTableNames );
        MultiDisplayNames = new Dictionary<short, string>( dto.MultiDisplayNames );
        MultiProductTableNames = new Dictionary<short, string>( dto.MultiProductTableNames );
        
        // MULTI CATEGORIES
        var multiTablesCategorySets = new List<IReadOnlySet<short>>();
        Dictionary<short, HashSet<short>>.KeyCollection multiCategoryKeys = dto.MultiTableCategories.Keys;
        var multiCategoriesDict = new Dictionary<short, IReadOnlySet<short>>();
        foreach ( short category in multiCategoryKeys )
            multiTablesCategorySets.Add( new HashSet<short>( dto.MultiTableCategories[ category ] ) );
        int multiCategoryCount = 0;
        foreach ( short category in multiCategoryKeys )
        {
            multiCategoriesDict.Add( category, multiTablesCategorySets[ multiCategoryCount ] );
            multiCategoryCount++;
        }
        MultiTablesByCategory = multiCategoriesDict;
        
        // MULTI VALUES
        var multiTablesValueSets = new List<IReadOnlyList<string>>();
        Dictionary<short, List<string>>.KeyCollection multiValuesKeys = dto.MultiValuesByTable.Keys;
        var multiValuesDict = new Dictionary<short, IReadOnlyList<string>>();
        foreach ( short id in multiValuesKeys )
            multiTablesValueSets.Add( new List<string>( dto.MultiValuesByTable[ id ] ) );
        int multiValueCount = 0;
        foreach ( short id in multiValuesKeys )
        {
            multiValuesDict.Add( id, multiTablesValueSets[ multiValueCount ] );
            multiValueCount++;
        }
        MultiValuesByTable = multiValuesDict;
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
    
    public IReadOnlyDictionary<short, IReadOnlyList<string>> IntFilterNames { get; init; }
    public IReadOnlyDictionary<short, IReadOnlyList<string>> StringFilterNames { get; init; }

    public IReadOnlySet<short> MultiGlobalTableIds { get; init; }
    public IReadOnlyDictionary<short, string> MultiTableNames { get; init; }
    public IReadOnlyDictionary<short, string> MultiDisplayNames { get; init; }
    public IReadOnlyDictionary<short, string> MultiProductTableNames { get; init; }
    public IReadOnlyDictionary<short, IReadOnlySet<short>> MultiTablesByCategory { get; init; }
    public IReadOnlyDictionary<short, IReadOnlyList<string>> MultiValuesByTable { get; init; }
}