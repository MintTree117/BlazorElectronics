using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Shared.Categories;

public sealed class CategoriesResponse
{
    public CategoriesResponse()
    {
        
    }
    public CategoriesResponse( Dictionary<int, CategoryModel> categories, List<int> primary, Dictionary<string, int> urls )
    {
        CategoriesById = categories;
        PrimaryIds = primary;
        Urls = urls;
    }
    
    public Dictionary<int, CategoryModel> CategoriesById { get; init; } = new();
    public List<int> PrimaryIds { get; init; } = new();
    public Dictionary<string, int> Urls { get; init; } = new();

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

            if ( !CategoriesById.TryGetValue( id, out CategoryModel? r ) )
                return false;

            if ( r.Tier != ( CategoryTier ) count )
                return false;

            categoryId = id;
            count++;
        }

        return true;
    }
}