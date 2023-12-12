using BlazorElectronics.Server.Core.Models.Sessions;
using BlazorElectronics.Server.Core.Models.Users;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface ISessionService
{
    Task<ServiceReply<SessionDto?>> CreateSession( int userId, UserDeviceInfoDto? deviceInfo );
    Task<ServiceReply<bool>> DeleteSession( int sessionId );
    Task<ServiceReply<bool>> AuthorizeSession( int sessionId, string sessionToken, UserDeviceInfoDto? deviceInfo );
    Task<ServiceReply<int>> AuthorizeSessionId( int sessionId, string sessionToken, UserDeviceInfoDto? deviceInfo );
}