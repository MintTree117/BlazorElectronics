using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Server.Dtos.Categories;

public sealed class CategoryUrlMap : LocallyCachedObject
{
    IReadOnlyDictionary<string, short> PrimaryUrlMap { get; }
    IReadOnlyDictionary<string, IReadOnlyDictionary<short, short>> SecondaryUrlMap { get; }
    IReadOnlyDictionary<string, IReadOnlyDictionary<short, short>> TertiaryUrlMap { get; }

    public CategoryUrlMap( Dictionary<string, short> primary, Dictionary<string, Dictionary<short, short>> secondary, Dictionary<string, Dictionary<short, short>> tertiary )
    {
        PrimaryUrlMap = new Dictionary<string, short>( primary );

        var secondaryTempMaps = new Dictionary<string, IReadOnlyDictionary<short, short>>();
        var tertiaryTempMaps = new Dictionary<string, IReadOnlyDictionary<short, short>>();

        foreach ( string url in secondary.Keys )
        {
            Dictionary<short, short> secondaryDict = secondary[ url ];
            secondaryTempMaps.Add( url, new Dictionary<short, short>( secondaryDict ) );
        }
        foreach ( string url in tertiary.Keys )
        {
            Dictionary<short, short> tertiaryDict = tertiary[ url ];
            tertiaryTempMaps.Add( url, new Dictionary<short, short>( tertiaryDict ) );
        }

        SecondaryUrlMap = secondaryTempMaps;
        TertiaryUrlMap = tertiaryTempMaps;
    }
    
    public CategoryIdMap? GetCategoryIdMapFromUrl( List<string> urlCategories )
    {
        return urlCategories.Count switch {
            1 => TryGetPrimaryTier( urlCategories[ 0 ] ),
            2 => TryGetSecondaryTier( urlCategories[ 0 ], urlCategories[ 1 ] ),
            3 => TryGetTertiaryTier( urlCategories[ 0 ], urlCategories[ 1 ], urlCategories[ 2 ] ),
            _ => null
        };
    }

    CategoryIdMap? TryGetPrimaryTier( string primaryUrl )
    {
        return PrimaryUrlMap.TryGetValue( primaryUrl, out short primaryId )
            ? new CategoryIdMap( 1, primaryId )
            : null;
    }
    CategoryIdMap? TryGetSecondaryTier( string primaryUrl, string secondaryUrl )
    {
        if ( !PrimaryUrlMap.TryGetValue( primaryUrl, out short primaryId ) )
            return null;

        if ( !SecondaryUrlMap.TryGetValue( secondaryUrl, out IReadOnlyDictionary<short, short>? secondaryMap ) )
            return null;
        
        return secondaryMap.TryGetValue( primaryId, out short secondaryId )
            ? new CategoryIdMap( 2, secondaryId )
            : null;
    }
    CategoryIdMap? TryGetTertiaryTier( string primaryUrl, string secondaryUrl, string tertiaryUrl )
    {
        if ( !PrimaryUrlMap.TryGetValue( primaryUrl, out short primaryId ) )
            return null;

        if ( !SecondaryUrlMap.TryGetValue( secondaryUrl, out IReadOnlyDictionary<short, short>? secondaryMap ) )
            return null;

        if ( !secondaryMap.TryGetValue( primaryId, out short secondaryId ) )
            return null;

        if ( !TertiaryUrlMap.TryGetValue( tertiaryUrl, out IReadOnlyDictionary<short, short>? tertiaryMap ) )
            return null;

        return tertiaryMap.TryGetValue( secondaryId, out short tertiaryId )
            ? new CategoryIdMap( 3, tertiaryId )
            : null;
    }
}