namespace BlazorElectronics.Server.Core.Services;

public abstract class _ApiService
{
    protected const string NO_DATA_FOUND_MESSAGE = "No data was found in cache or database!";

    protected readonly ILogger<_ApiService> Logger;

    protected _ApiService( ILogger<_ApiService> logger )
    {
        Logger = logger;
    }
}