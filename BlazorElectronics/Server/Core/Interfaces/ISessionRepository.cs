using BlazorElectronics.Server.Core.Models.Users;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface ISessionRepository
{
    Task<UserSession?> InsertSession( int userId, byte[] sessionHash, byte[] sessionSalt, UserDeviceInfoDto? deviceInfo );
    Task<bool> DeleteSession( int sessionId );
    Task<UserSession?> GetSession( int sessionId );
}