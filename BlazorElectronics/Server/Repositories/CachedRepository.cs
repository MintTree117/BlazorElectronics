using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace BlazorElectronics.Server.Repositories;

public abstract class CachedRepository
{
    protected readonly IDistributedCache MemoryCache;

    public CachedRepository( IDistributedCache memoryCache )
    {
        MemoryCache = memoryCache;
    }

    protected async Task Cache<T>( string key, T data, DistributedCacheEntryOptions entryOptions ) where T : class?
    {
        await Task.Run( () =>
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes( data );
            MemoryCache.Set( key, bytes, entryOptions );
        } );
    }
    protected async Task<T?> GetFromCache<T>( string key, DistributedCacheEntryOptions entryOptions ) where T : class?
    {
        var specs = await MemoryCache.GetAsync( key );

        if ( specs == null )
            return null;

        await using var stream = new MemoryStream( specs );
        return await JsonSerializer.DeserializeAsync<T>( stream );
    }
    protected async Task<T?> GetFromRepository<T>( string key, Task<T> repositoryTask, DistributedCacheEntryOptions entryOptions ) where T : class?
    {
        T? repoSpecs = await repositoryTask;

        if ( repoSpecs == null )
            return null;

        await Task.Run( () =>
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes( repoSpecs );
            MemoryCache.Set( key, bytes, entryOptions );
        } );

        return repoSpecs;
    }
}