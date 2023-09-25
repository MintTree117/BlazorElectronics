using Microsoft.Extensions.Caching.Memory;

namespace BlazorElectronics.Server.Services.Products;

public sealed class ProductCache : CacheService, IProductCache
{
    public ProductCache( IMemoryCache memoryCache ) : base( memoryCache ) { }
}