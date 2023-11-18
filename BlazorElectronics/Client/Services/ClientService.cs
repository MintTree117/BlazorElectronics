namespace BlazorElectronics.Client.Services;

public abstract class ClientService<T>
{
    protected readonly ILogger<T> Logger;
    
    protected ClientService( ILogger<T> logger )
    {
        Logger = logger;
    }
}