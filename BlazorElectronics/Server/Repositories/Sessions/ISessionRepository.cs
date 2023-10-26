using BlazorElectronics.Server.Models.Users;

namespace BlazorElectronics.Server.Repositories.Sessions;

public interface ISessionRepository
{
    Task<UserSession?> CreateSession( UserSession session );
    Task<UserSession?> GetSession( int userId, string ipAddress );
    Task<bool> UpdateSession( UserSession update );
}