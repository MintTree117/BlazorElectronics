using BlazorElectronics.Shared.DataTransferObjects.Categories;
using Microsoft.Extensions.Caching.Distributed;

namespace BlazorElectronics.Server.Services.Categories;

public sealed class CategoryCache : CachedService, ICategoryCache
{
    const string CACHE_KEY_CATEGORIES = "Categories";

    public CategoryCache( IDistributedCache memoryCache ) : base( memoryCache ) { }

    public async Task<Categories_DTO?> Get()
    {
        return await GetFromCache<Categories_DTO>( CACHE_KEY_CATEGORIES );
    }
    public async Task Set( Categories_DTO dto )
    {
        await Cache( CACHE_KEY_CATEGORIES, dto, new DistributedCacheEntryOptions()
            .SetSlidingExpiration( TimeSpan.FromHours( 1.0 ) )
            .SetAbsoluteExpiration( TimeSpan.FromDays( 1 ) ) );
    }
}