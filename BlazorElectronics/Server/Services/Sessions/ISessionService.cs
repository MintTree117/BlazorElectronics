namespace BlazorElectronics.Server.Services.Sessions;

public interface ISessionService
{
    Task<ServiceResponse<string?>> CreateNewSession( int userId, string ipAddress );
    Task<ServiceResponse<string?>> GetExistingSession( int userId, string sessionToken, string ipAddress );
}