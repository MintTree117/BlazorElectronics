namespace BlazorElectronics.Server.Services.Sessions;

public interface ISessionService
{
    Task<Reply<string?>> CreateNewSession( int userId, string ipAddress );
    Task<Reply<string?>> GetExistingSession( int userId, string sessionToken, string ipAddress );
}