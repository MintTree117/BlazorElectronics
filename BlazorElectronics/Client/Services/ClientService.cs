namespace BlazorElectronics.Client.Services;

public abstract class ClientService
{
    protected readonly ILogger<ClientService> Logger;
    
    protected ClientService( ILogger<ClientService> logger )
    {
        Logger = logger;
    }
}