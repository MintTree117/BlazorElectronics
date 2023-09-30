using BlazorElectronics.Server.Models.Specs;

namespace BlazorElectronics.Server.Services.Specs;

public interface ISpecCache
{
    Task<CachedSpecDescrs?> GetSpecDescrs();
    Task<CachedSpecLookups?> GetSpecLookups();

    Task CacheSpecDescrs( CachedSpecDescrs dto );
    Task CacheSpecLookups( CachedSpecLookups dto );
}