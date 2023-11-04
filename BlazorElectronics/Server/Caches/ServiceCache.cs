using System.Security;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace BlazorElectronics.Server.Caches;

public abstract class ServiceCache 
{
    readonly IDistributedCache MemoryCache;

    protected ServiceCache( IDistributedCache memoryCache ) { MemoryCache = memoryCache; }

    protected async Task Cache<T>( string key, T data, DistributedCacheEntryOptions options ) where T : class?
    {
        try
        {
            await SerializeAndCache( key, data, options );
        }
        catch ( Exception ex )
        {
            throw new ServiceException( ex.Message, ex );
        }
    }
    protected async Task<T?> GetFromCache<T>( string key ) where T : class?
    {
        try
        {
            return await DeserializeAndReturn<T>( key );
        }
        catch ( Exception ex )
        {
            throw new SecurityException( ex.Message, ex );
        }
    }

    async Task SerializeAndCache<T>( string key, T data, DistributedCacheEntryOptions options ) where T : class?
    {
        await using var memoryStream = new MemoryStream();
        
        await JsonSerializer.SerializeAsync( memoryStream, data );
        byte[] bytes = memoryStream.ToArray();
        
        await MemoryCache.SetAsync( key, bytes, options );
    }
    async Task<T?> DeserializeAndReturn<T>( string key ) where T : class?
    {
        byte[]? bytes = await MemoryCache.GetAsync( key );

        if ( bytes == null )
            return null;

        await using var stream = new MemoryStream( bytes );
        return await JsonSerializer.DeserializeAsync<T>( stream );
    }
}