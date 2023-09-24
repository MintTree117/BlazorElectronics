using System.Collections.Concurrent;
using BlazorElectronics.Server.Models.Categories;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorElectronics.Server.Services.Categories;

public class CategoryCache : CacheService, ICategoryCache
{
    const string CATEGORY_KEY = "Categories";
    ConcurrentDictionary<string, int>? _urlToCategoryId;

    public CategoryCache( IMemoryCache cache ) : base( cache ) { }

    public async Task<List<Category>?> GetCategories()
    {
        return await Task.Run( () => _cache.TryGetValue( CATEGORY_KEY, out List<Category>? categories ) ? categories : null );
    }
    public async Task CacheCategories( List<Category> categories )
    {
        await Task.Run( () => {
            _urlToCategoryId = new ConcurrentDictionary<string, int>();
            foreach ( Category c in categories )
                _urlToCategoryId.TryAdd( c.CategoryUrl, c.CategoryId );
            _cache.Set( "Categories", categories );
        } );
    }
    public bool HasCategoryUrls()
    {
        return _urlToCategoryId != null;
    }
    public bool UrlToCategoryId( string categoryUrl, out int id )
    {
        id = -1;
        return _urlToCategoryId != null && 
               _urlToCategoryId.TryGetValue( categoryUrl, out id );
    }
}