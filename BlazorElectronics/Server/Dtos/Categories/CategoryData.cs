using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Server.Dtos.Categories;

public sealed class CategoryData
{
    public CategoryData( CategoriesResponse response, Dictionary<string, int> urls )
    {
        Response = response;
        Urls = urls;
    }
    public CategoriesResponse Response { get; init; }
    public Dictionary<string, int> Urls { get; init; }

    public bool ValidateUrl( string url, out int categoryId )
    {
        categoryId = -1;
        string[] urls = url.Split( "/" );

        if ( urls.Length is <= 0 or > 3 )
            return false;
        
        int count = 1;
        
        foreach ( string u in urls )
        {
            if ( !Urls.TryGetValue( u, out int id ) )
                return false;

            if ( !Response.CategoriesById.TryGetValue( id, out CategoryResponse? r ) )
                return false;

            if ( r.Tier != ( CategoryTier ) count )
                return false;

            categoryId = id;
            count++;
        }
        
        return true;
    }
}