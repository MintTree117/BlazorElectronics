using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace BlazorElectronics.Server.Services;

public abstract class CachedService
{
    readonly IDistributedCache MemoryCache;

    protected CachedService( IDistributedCache memoryCache ) { MemoryCache = memoryCache; }

    protected async Task Cache<T>( string key, T data, DistributedCacheEntryOptions entryOptions ) where T : class?
    {
        await Task.Run( () =>
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes( data );
            MemoryCache.Set( key, bytes, entryOptions );
        } );
    }
    protected async Task<T?> GetFromCache<T>( string key ) where T : class?
    {
        var specs = await MemoryCache.GetAsync( key );

        if ( specs == null )
            return null;

        await using var stream = new MemoryStream( specs );
        return await JsonSerializer.DeserializeAsync<T>( stream );
    }
}