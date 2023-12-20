using Blazored.LocalStorage;
using BlazorElectronics.Client.Models;

namespace BlazorElectronics.Client.Services;

public abstract class CachedClientService<T> : ClientService where T : class
{
    readonly int CACHE_LIFE_HOURS;
    readonly string CACHE_KEY;

    protected CachedClientService( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage, int cacheLifeHours, string cacheKey )
        : base( logger, http, storage )
    {
        CACHE_LIFE_HOURS = cacheLifeHours;
        CACHE_KEY = cacheKey;
    }

    protected async Task<T?> TryGetCachedItem()
    {
        var item = await Storage.GetItemAsync<BrowserCacheItem<T>>( CACHE_KEY );

        if ( item is null )
            return null;

        if ( item.IsValid( CACHE_LIFE_HOURS ) )
            return item.Data;

        await Storage.RemoveItemAsync( CACHE_KEY );
        return null;
    }
    protected async Task TrySetCachedItem( T item )
    {
        BrowserCacheItem<T> cache = new( item );
        await Storage.SetItemAsync( CACHE_KEY, cache );
    }
}