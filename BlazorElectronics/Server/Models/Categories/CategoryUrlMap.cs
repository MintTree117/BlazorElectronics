using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Server.Models.Categories;

public sealed class CategoryUrlMap
{
    IReadOnlyDictionary<string, short> PrimaryUrlMap { get; set; }
    IReadOnlyDictionary<string, IReadOnlyDictionary<short, short>> SecondaryUrlMap { get; set; }
    IReadOnlyDictionary<string, IReadOnlyDictionary<short, short>> TertiaryUrlMap { get; set; }

    public CategoryUrlMap( Dictionary<string, short> primary, Dictionary<string, IReadOnlyDictionary<short, short>> secondary, IReadOnlyDictionary<string, IReadOnlyDictionary<short, short>> tertiary )
    {
        PrimaryUrlMap = primary;
        SecondaryUrlMap = secondary;
        TertiaryUrlMap = tertiary;
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