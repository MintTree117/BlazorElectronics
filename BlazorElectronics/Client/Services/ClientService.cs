namespace BlazorElectronics.Client.Services;

public abstract class ClientService
{
    protected const string ERROR_NULL_LOCAL_SESSION = "Failed to get local user session!";
    
    protected readonly ILogger<ClientService> Logger;
    
    protected ClientService( ILogger<ClientService> logger )
    {
        Logger = logger;
    }
}