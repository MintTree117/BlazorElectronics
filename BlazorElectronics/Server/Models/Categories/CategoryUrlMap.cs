using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Server.Models.Categories;

public sealed class CategoryUrlMap
{
    public Dictionary<string, int> PrimaryUrlMap { get; set; } = new();
    public Dictionary<string, Dictionary<int, int>> SecondaryUrlMap { get; set; } = new();
    public Dictionary<string, Dictionary<int, int>> TertiaryUrlMap { get; set; } = new();

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
            ? new CategoryIdMap( 1, primaryId )
            : null;
    }
    CategoryIdMap? TryGetSecondaryTier( string primaryUrl, string secondaryUrl )
    {
        if ( !PrimaryUrlMap.TryGetValue( primaryUrl, out int primaryId ) )
            return null;

        if ( !SecondaryUrlMap.TryGetValue( secondaryUrl, out Dictionary<int, int>? secondaryMap ) )
            return null;

        return secondaryMap.TryGetValue( primaryId, out int secondaryId )
            ? new CategoryIdMap( 2, secondaryId )
            : null;
    }
    CategoryIdMap? TryGetTertiaryTier( string primaryUrl, string secondaryUrl, string tertiaryUrl )
    {
        if ( !PrimaryUrlMap.TryGetValue( primaryUrl, out int primaryId ) )
            return null;

        if ( !SecondaryUrlMap.TryGetValue( secondaryUrl, out Dictionary<int, int>? secondaryMap ) )
            return null;

        if ( !secondaryMap.TryGetValue( primaryId, out int secondaryId ) )
            return null;

        if ( !TertiaryUrlMap.TryGetValue( tertiaryUrl, out Dictionary<int, int>? tertiaryMap ) )
            return null;

        return tertiaryMap.TryGetValue( secondaryId, out int tertiaryId )
            ? new CategoryIdMap( 3, tertiaryId )
            : null;
    }
}