using BlazorElectronics.Server.Services.Specs;
using BlazorElectronics.Shared.Outbound.Specs;
using Microsoft.Extensions.Caching.Distributed;

namespace BlazorElectronics.Server.Caches.Specs;

public class SpecLookupCache : ServiceCache, ISpecLookupCache
{
    const string CACHE_KEY_LOOKUPS_GLOBAL = "SpecLookupsGlobal";
    const string CACHE_KEY_PATTERN_LOOKUPS_CATEGORY = "SpecLookupsCategory";
    
    public SpecLookupCache( IDistributedCache memoryCache )
        : base( memoryCache ) { }
    
    public async Task<SpecLookupsGlobalResponse?> GetSpecLookupsGlobal()
    {
        return await GetFromCache<SpecLookupsGlobalResponse?>( CACHE_KEY_LOOKUPS_GLOBAL );
    }
    public async Task<SpecsLookupsCategoryResponse?> GetSpecLookupsCategory( int primaryCategoryId )
    {
        string key = GetCategoryLookupCacheKey( primaryCategoryId );
        return await GetFromCache<SpecsLookupsCategoryResponse?>( key );
    }
    public async Task SetSpecLookupsGlobal( SpecLookupsGlobalResponse response )
    {
        await Cache( CACHE_KEY_LOOKUPS_GLOBAL, response, new DistributedCacheEntryOptions() );
    }
    public async Task SetSpecLookupsCategory( int primaryCategoryId, SpecsLookupsCategoryResponse response )
    {
        string key = GetCategoryLookupCacheKey( primaryCategoryId );
        await Cache( key, response, new DistributedCacheEntryOptions() );
    }

    static string GetCategoryLookupCacheKey( int id )
    {
        return CACHE_KEY_PATTERN_LOOKUPS_CATEGORY + id;
    }
}