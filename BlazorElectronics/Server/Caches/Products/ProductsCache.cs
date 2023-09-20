using Microsoft.Extensions.Caching.Memory;

namespace BlazorElectronics.Server.Caches.Products;

public sealed class ProductCache : IProductCache
{
    readonly MemoryCache cache = new MemoryCache( new MemoryCacheOptions() );
    
    
}