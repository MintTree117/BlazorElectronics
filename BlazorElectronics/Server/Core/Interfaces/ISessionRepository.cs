using BlazorElectronics.Server.Core.Models.Sessions;
using BlazorElectronics.Server.Core.Models.Users;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface ISessionRepository
{
    Task<SessionModel?> InsertSession( int userId, byte[] sessionHash, byte[] sessionSalt, UserDeviceInfoDto? deviceInfo );
    Task<bool> DeleteSession( int sessionId );
    Task<bool> DeleteAllSessions( int userId );
    Task<SessionModel?> GetSession( int sessionId );
    Task<IEnumerable<SessionModel>?> GetSessions( int userId );
    Task<SessionValidationModel?> GetSessionValidation( int sessionId );
}