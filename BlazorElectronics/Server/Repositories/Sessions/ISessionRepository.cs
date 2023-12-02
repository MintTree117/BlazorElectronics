using BlazorElectronics.Server.Dtos.Users;
using BlazorElectronics.Server.Models.Users;

namespace BlazorElectronics.Server.Repositories.Sessions;

public interface ISessionRepository
{
    Task<UserSession?> InsertSession( int userId, byte[] sessionHash, byte[] sessionSalt, UserDeviceInfoDto? deviceInfo );
    Task<bool> DeleteSession( int sessionId );
    Task<UserSession?> GetSession( int sessionId );
}