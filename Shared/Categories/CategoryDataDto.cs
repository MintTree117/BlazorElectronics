using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Shared.Categories;

public sealed class CategoryDataDto
{
    public CategoryDataDto()
    {
        
    }
    public CategoryDataDto( List<CategoryLightDto> responses )
    {
        foreach ( CategoryLightDto r in responses )
        {
            CategoriesById.Add( r.Id, new CategoryFullDto
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
    public CategoryDataDto( List<CategoryFullDto> models )
    {
        foreach ( CategoryFullDto m in models )
        {
            CategoriesById.Add( m.CategoryId, m );

            if ( m.Tier == CategoryTier.Primary )
                PrimaryIds.Add( m.CategoryId );
        }
        
        BuildData();
    }
    
    public Dictionary<int, CategoryFullDto> CategoriesById { get; init; } = new();
    public List<int> PrimaryIds { get; init; } = new();
    public Dictionary<string, int> Urls { get; init; } = new();

    void BuildData()
    {
        Dictionary<int, CategoryFullDto>.ValueCollection models = CategoriesById.Values;
        
        foreach ( CategoryFullDto m in models )
        {
            if ( m.ParentCategoryId is null or null )
                continue;

            if ( CategoriesById.TryGetValue( m.ParentCategoryId.Value, out CategoryFullDto? parent ) )
                parent.Children.Add( m );
        }

        // Create a new dictionary to hold the full URLs.
        var fullUrls = new Dictionary<int, string>();

        foreach ( CategoryFullDto m in models )
        {
            // Use the refactored method to build the full URL and store it in the new dictionary.
            fullUrls[ m.CategoryId ] = BuildApiUrl( m, CategoriesById );
        }

        foreach ( CategoryFullDto m in models )
        {
            // Update the model's ApiUrl with the full URL from the dictionary.
            m.ApiUrl = fullUrls[ m.CategoryId ];

            // Continue with adding to Urls dictionary.
            Urls.TryAdd( m.ApiUrl, m.CategoryId );
        }
    }
    static string BuildApiUrl( CategoryFullDto m, Dictionary<int, CategoryFullDto> responses )
    {
        // Base case: If no parent, return the category's own URL.
        if ( m.ParentCategoryId == null )
        {
            return m.ApiUrl;
        }

        // If the category has a parent, recursively get the parent's URL.
        if ( responses.TryGetValue( m.ParentCategoryId.Value, out CategoryFullDto? parent ) )
        {
            string parentUrl = BuildApiUrl( parent, responses );

            // Construct the full URL by combining parent URL with the current URL.
            return $"{parentUrl}/{m.ApiUrl}";
        }

        // If no parent is found in the responses, return the current URL.
        return m.ApiUrl;
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

            if ( !CategoriesById.TryGetValue( id, out CategoryFullDto? r ) )
                return false;

            if ( r.Tier != ( CategoryTier ) count )
                return false;

            categoryId = id;
            count++;
        }

        return true;
    }
    public List<CategoryFullDto> GetPrimaryCategories()
    {
        return ( from id in PrimaryIds select CategoriesById[ id ] ).ToList();
    }
}