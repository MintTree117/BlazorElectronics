using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Server.Dtos.Categories;

public sealed class CategoryUrlMap
{
    IReadOnlyDictionary<string, int> PrimaryUrlMap { get; }
    IReadOnlyDictionary<string, IReadOnlyDictionary<int, int>> SecondaryUrlMap { get; }
    IReadOnlyDictionary<string, IReadOnlyDictionary<int, int>> TertiaryUrlMap { get; }
    
    public CategoryUrlMap( Dictionary<string, int> primary, Dictionary<string, Dictionary<int, int>> secondary, Dictionary<string, Dictionary<int, int>> tertiary )
    {
        PrimaryUrlMap = new Dictionary<string, int>( primary );
        
        var secondaryTempMaps = new Dictionary<string, IReadOnlyDictionary<int, int>>();
        var tertiaryTempMaps = new Dictionary<string, IReadOnlyDictionary<int, int>>();

        foreach ( string url in secondary.Keys )
        {
            Dictionary<int, int> secondaryDict = secondary[ url ];
            secondaryTempMaps.Add( url, new Dictionary<int, int>( secondaryDict ) );
        }
        foreach ( string url in tertiary.Keys )
        {
            Dictionary<int, int> tertiaryDict = tertiary[ url ];
            tertiaryTempMaps.Add( url, new Dictionary<int, int>( tertiaryDict ) );
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
        return PrimaryUrlMap.TryGetValue( primaryUrl, out int primaryId )
            ? new CategoryIdMap( CategoryType.PRIMARY, primaryId )
            : null;
    }
    CategoryIdMap? TryGetSecondaryTier( string primaryUrl, string secondaryUrl )
    {
        if ( !PrimaryUrlMap.TryGetValue( primaryUrl, out int primaryId ) )
            return null;

        if ( !SecondaryUrlMap.TryGetValue( secondaryUrl, out IReadOnlyDictionary<int, int>? secondaryMap ) )
            return null;
        
        return secondaryMap.TryGetValue( primaryId, out int secondaryId )
            ? new CategoryIdMap( CategoryType.SECONDARY, secondaryId )
            : null;
    }
    CategoryIdMap? TryGetTertiaryTier( string primaryUrl, string secondaryUrl, string tertiaryUrl )
    {
        if ( !PrimaryUrlMap.TryGetValue( primaryUrl, out int primaryId ) )
            return null;

        if ( !SecondaryUrlMap.TryGetValue( secondaryUrl, out IReadOnlyDictionary<int, int>? secondaryMap ) )
            return null;

        if ( !secondaryMap.TryGetValue( primaryId, out int secondaryId ) )
            return null;

        if ( !TertiaryUrlMap.TryGetValue( tertiaryUrl, out IReadOnlyDictionary<int, int>? tertiaryMap ) )
            return null;

        return tertiaryMap.TryGetValue( secondaryId, out int tertiaryId )
            ? new CategoryIdMap( CategoryType.TERTIARY, tertiaryId )
            : null;
    }
}