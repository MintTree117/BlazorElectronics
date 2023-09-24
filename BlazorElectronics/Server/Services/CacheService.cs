using Microsoft.Extensions.Caching.Memory;

namespace BlazorElectronics.Server.Services;

public abstract class CacheService
{
    protected readonly IMemoryCache _cache;

    public CacheService( IMemoryCache cache )
    {
        _cache = cache;
    }
}