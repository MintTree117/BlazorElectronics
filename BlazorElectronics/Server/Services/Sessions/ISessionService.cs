using BlazorElectronics.Server.Dtos.Sessions;
using BlazorElectronics.Server.Dtos.Users;

namespace BlazorElectronics.Server.Services.Sessions;

public interface ISessionService
{
    Task<ApiReply<SessionDto?>> CreateSession( int userId, UserDeviceInfoDto? deviceInfo );
    Task<ApiReply<bool>> DeleteSession( int sessionId );
    Task<ApiReply<int>> AuthorizeSession( int sessionId, string sessionToken, UserDeviceInfoDto? deviceInfo );
}