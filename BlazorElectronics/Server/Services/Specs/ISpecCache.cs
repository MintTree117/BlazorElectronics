using BlazorElectronics.Server.Models.Specs;

namespace BlazorElectronics.Server.Services.Specs;

public interface ISpecCache
{
    Task<CachedSpecDescrs?> GetSpecDescrs( int categoryId );
    Task<CachedSpecLookups?> GetSpecLookups( int categoryId );

    Task CacheSpecDescrs( CachedSpecDescrs dto, int categoryId );
    Task CacheSpecLookups( CachedSpecLookups dto, int categoryId );
}