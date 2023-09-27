using BlazorElectronics.Server.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorElectronics.Server.Services.Products;

public sealed class ProductCache : CachedRepository, IProductCache
{
    public ProductCache( IDistributedCache memoryCache ) : base( memoryCache ) { }
}