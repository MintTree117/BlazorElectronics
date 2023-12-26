namespace BlazorElectronics.Server.Core.Interfaces;

public interface IDapperRepository
{ 
    Task<DateTime> PingCacheTable( string cacheName );
}