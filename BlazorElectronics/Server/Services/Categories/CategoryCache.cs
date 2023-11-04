using BlazorElectronics.Server.Caches;
using BlazorElectronics.Server.Models.Categories;
using BlazorElectronics.Shared.Outbound.Categories;
using Microsoft.Extensions.Caching.Distributed;

namespace BlazorElectronics.Server.Services.Categories;

public sealed class CategoryCache : ServiceCache, ICategoryCache
{
    const string CACHE_KEY_DESCRIPTIONS_PRIMARY = "DescriptionsPrimary";
    const string CACHE_KEY_CATEGORIES_RESPONSE = "CategoriesResponse";
    const string CACHE_KEY_URL_MAP = "CategoryMap";

    const int SLIDING_EXPIRATION_PRIMARY_DESCRIPTIONS = 24;
    const int ABSOLUTE_EXPIRATION_PRIMARY_DESCRIPTIONS = 24;
    const int SLIDING_EXPIRATION_CATEGORIES_RESPONSE = 24;
    const int SLIDING_EXPIRATION_URL_MAP = 24;
    const int ABSOLUTE_EXPIRATION_CATEGORIES_RESPONSE = 48;
    const int ABSOLUTE_EXPIRATION_URL_MAP = 48;

    public CategoryCache( IDistributedCache memoryCache ) : base( memoryCache ) { }

    public async Task<List<string?>?> GetPrimaryDescriptions()
    {
        try
        {
            return await GetFromCache<List<string?>>( CACHE_KEY_DESCRIPTIONS_PRIMARY );
        }
        catch ( Exception e )
        {
            throw new ServiceException( e.Message, e );
        }
    }
    public async Task<CategoriesResponse?> GetCategoriesResponse()
    {
        try
        {
            return await GetFromCache<CategoriesResponse?>( CACHE_KEY_CATEGORIES_RESPONSE );
        }
        catch ( Exception e )
        {
            throw new ServiceException( e.Message, e );
        }
    }
    public async Task<CategoryUrlMap?> GetUrlMap()
    {
        try
        {
            return await GetFromCache<CategoryUrlMap?>( CACHE_KEY_URL_MAP );
        }
        catch ( Exception e )
        {
            throw new ServiceException( e.Message, e );
        }
    }
    
    public async Task SetPrimaryDescriptions( List<string?> descriptions )
    {
        try
        {
            await Cache( CACHE_KEY_DESCRIPTIONS_PRIMARY, descriptions, new DistributedCacheEntryOptions()
                .SetSlidingExpiration( TimeSpan.FromHours( SLIDING_EXPIRATION_PRIMARY_DESCRIPTIONS ) )
                .SetAbsoluteExpiration( TimeSpan.FromDays( ABSOLUTE_EXPIRATION_PRIMARY_DESCRIPTIONS ) ) );
        }
        catch ( Exception e )
        {
            throw new ServiceException( e.Message, e );
        }
    }
    public async Task SetCategoriesResponse( CategoriesResponse response )
    {
        try
        {
            await Cache( CACHE_KEY_CATEGORIES_RESPONSE, response, new DistributedCacheEntryOptions()
                .SetSlidingExpiration( TimeSpan.FromHours( SLIDING_EXPIRATION_CATEGORIES_RESPONSE ) )
                .SetAbsoluteExpiration( TimeSpan.FromDays( ABSOLUTE_EXPIRATION_CATEGORIES_RESPONSE ) ) );
        }
        catch ( Exception e )
        {
            throw new ServiceException( e.Message, e );
        }
    }
    public async Task SetUrlMap( CategoryUrlMap map )
    {
        try
        {
            await Cache( CACHE_KEY_CATEGORIES_RESPONSE, map, new DistributedCacheEntryOptions()
                .SetSlidingExpiration( TimeSpan.FromHours( SLIDING_EXPIRATION_URL_MAP ) )
                .SetAbsoluteExpiration( TimeSpan.FromDays( ABSOLUTE_EXPIRATION_URL_MAP ) ) );
        }
        catch ( Exception e )
        {
            throw new ServiceException( e.Message, e );
        }
    }
}