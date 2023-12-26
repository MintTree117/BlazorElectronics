using BlazorElectronics.Server.Core.Interfaces;

namespace BlazorElectronics.Server.Core.Services;

public abstract class _CachedApiService : _ApiService
{
    readonly IDapperRepository _repo;
    readonly int _cacheLifeHours;
    readonly string _cacheName;
    readonly object _cacheLock = new();
    Timer? _timer;
    DateTime _lastCacheDate = DateTime.Now;

    protected _CachedApiService( ILogger<_ApiService> logger, IDapperRepository repo, int cacheLifeHours, string cacheName )
        : base( logger )
    {
        _repo = repo;
        _cacheLifeHours = cacheLifeHours;
        _cacheName = cacheName;
        ResetTimer();
    }

    protected abstract void UpdateCache();
    async void CheckForUpdates( object? state )
    {
        DateTime date = await _repo.PingCacheTable( _cacheName );

        if ( !( ( date - _lastCacheDate ).TotalHours < _cacheLifeHours ) )
            return;

        lock ( _cacheLock )
        {
            UpdateCache();
        }

        _lastCacheDate = DateTime.Now;
        ResetTimer();
    }
    void ResetTimer()
    {
        _timer = new Timer( CheckForUpdates, null, TimeSpan.Zero, TimeSpan.FromHours( _cacheLifeHours ) );
    }
}