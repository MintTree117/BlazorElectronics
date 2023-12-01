using BlazorElectronics.Server.Dtos.Sessions;
using BlazorElectronics.Server.Dtos.Users;

namespace BlazorElectronics.Server.Services.Sessions;

public interface ISessionService
{
    Task<ServiceReply<SessionDto?>> CreateSession( int userId, UserDeviceInfoDto? deviceInfo );
    Task<ServiceReply<bool>> DeleteSession( int sessionId );
    Task<ServiceReply<int>> AuthorizeSession( int sessionId, string sessionToken, UserDeviceInfoDto? deviceInfo );
}