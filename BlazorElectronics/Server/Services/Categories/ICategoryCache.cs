using BlazorElectronics.Server.Models.Categories;

namespace BlazorElectronics.Server.Services.Categories;

public interface ICategoryCache
{
    Task CacheCategories( List<Category> categories );
    Task<List<Category>?> GetCategories();
    bool HasCategoryUrls();
    bool UrlToCategoryId( string categoryUrl, out int id );
}