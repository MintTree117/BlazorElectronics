using BlazorElectronics.Server.Models.Categories;
using Microsoft.Extensions.Caching.Distributed;
using IDistributedCache = Microsoft.Extensions.Caching.Distributed.IDistributedCache;

namespace BlazorElectronics.Server.Repositories.Categories;

public class CachedCategoryRepository : CachedRepository, ICategoryRepository
{
    const string CATEGORY_KEY = "Categories";

    ICategoryRepository _categoryRepository;

    public CachedCategoryRepository( IDistributedCache cache, ICategoryRepository categoryRepository ) : base( cache ) { _categoryRepository = categoryRepository; }
    
    public async Task<Dictionary<string, Category>?> GetCategories2()
    {
        var cachedCategories = await GetFromCache<Dictionary<string, Category>?>( CATEGORY_KEY, new DistributedCacheEntryOptions()
            .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
            .SetAbsoluteExpiration( TimeSpan.FromHours( 5.0 ) ) );

        if ( cachedCategories != null )
            return cachedCategories;
        
        cachedCategories = GetFromRepository( CATEGORY_KEY, _categoryRepository.GetCategories2(), new DistributedCacheEntryOptions()
            .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
            .SetAbsoluteExpiration( TimeSpan.FromHours( 5.0 ) ) )
    }
    public async Task<List<Category>?> GetCategories()
    {
        var cachedCategories = await GetFromCache<Category>( CATEGORY_KEY, new DistributedCacheEntryOptions()
            .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
            .SetAbsoluteExpiration( TimeSpan.FromHours( 5.0 ) ) );
    }
}