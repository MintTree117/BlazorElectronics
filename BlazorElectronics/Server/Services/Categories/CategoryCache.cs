using BlazorElectronics.Shared.DataTransferObjects.Categories;
using Microsoft.Extensions.Caching.Distributed;

namespace BlazorElectronics.Server.Services.Categories;

public sealed class CategoryCache : CachedService, ICategoryCache
{
    const string CACHE_KEY_CATEGORIES = "Categories";

    public CategoryCache( IDistributedCache memoryCache ) : base( memoryCache ) { }

    public async Task<Models.Categories.CategoryMeta?> Get()
    {
        return await GetFromCache<Models.Categories.CategoryMeta>( CACHE_KEY_CATEGORIES );
    }
    public async Task Set( Models.Categories.CategoryMeta dto )
    {
        await Cache( CACHE_KEY_CATEGORIES, dto, new DistributedCacheEntryOptions()
            .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
            .SetAbsoluteExpiration( TimeSpan.FromDays( 1 ) ) );
    }
}