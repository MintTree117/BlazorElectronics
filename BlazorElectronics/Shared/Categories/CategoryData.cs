using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Shared.Categories;

public sealed class CategoryData
{
    public CategoryData()
    {
        
    }
    public CategoryData( List<CategoryResponse> responses )
    {
        foreach ( CategoryResponse r in responses )
        {
            CategoriesById.Add( r.Id, new CategoryModel
            {
                CategoryId = r.Id,
                ParentCategoryId = r.ParentId,
                Name = r.Name,
                Tier = r.Tier,
                ApiUrl = r.Url,
                ImageUrl = r.ImageUrl
            } );

            if ( r.Tier == CategoryTier.Primary )
                PrimaryIds.Add( r.Id );
        }

        BuildData();
    }
    public CategoryData( List<CategoryModel> models )
    {
        foreach ( CategoryModel m in models )
        {
            CategoriesById.Add( m.CategoryId, m );

            if ( m.Tier == CategoryTier.Primary )
                PrimaryIds.Add( m.CategoryId );
        }
        
        BuildData();
    }
    
    public Dictionary<int, CategoryModel> CategoriesById { get; init; } = new();
    public List<int> PrimaryIds { get; init; } = new();
    public Dictionary<string, int> Urls { get; init; } = new();

    void BuildData()
    {
        Dictionary<int, CategoryModel>.ValueCollection models = CategoriesById.Values;
        
        foreach ( CategoryModel m in models )
        {
            if ( m.ParentCategoryId is null or null )
                continue;

            if ( CategoriesById.TryGetValue( m.ParentCategoryId.Value, out CategoryModel? parent ) )
                parent.Children.Add( m );
        }

        foreach ( CategoryModel m in models )
        {
            m.ApiUrl = BuildApiUrl( m, m.ApiUrl, CategoriesById );
        }

        foreach ( CategoryModel m in models )
        {
            Urls.TryAdd( m.ApiUrl, m.CategoryId );
        }
    }
    static string BuildApiUrl( CategoryModel m, string url, Dictionary<int, CategoryModel> responses )
    {
        while ( true )
        {
            if ( m.ParentCategoryId is null || !responses.TryGetValue( m.ParentCategoryId.Value, out CategoryModel? parent ) ) 
                return url;

            url = $"{parent.ApiUrl}/{url}";
            m = parent;
        }
    }
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
    public List<CategoryModel> GetPrimaryCategories()
    {
        return ( from id in PrimaryIds select CategoriesById[ id ] ).ToList();
    }
}