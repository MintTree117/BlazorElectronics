using Microsoft.Extensions.Caching.Memory;

namespace BlazorElectronics.Server.Services;

public abstract class CacheService
{
    protected readonly IMemoryCache MemoryCache;

    public CacheService( IMemoryCache memoryCache )
    {
        MemoryCache = memoryCache;
    }
}