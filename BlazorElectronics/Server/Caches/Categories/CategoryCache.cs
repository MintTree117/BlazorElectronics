using Microsoft.Extensions.Caching.Memory;

namespace BlazorElectronics.Server.Caches.Categories;

public class CategoryCache : ICategoryCache
{
    readonly MemoryCache cache = new MemoryCache( new MemoryCacheOptions() );
}