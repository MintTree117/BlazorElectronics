using BlazorElectronics.Server.Core.Models.Sessions;
using BlazorElectronics.Server.Core.Models.Users;
using BlazorElectronics.Shared.Users;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface ISessionService
{
    Task<ServiceReply<SessionDto?>> CreateSession( int userId, UserDeviceInfoDto? deviceInfo );
    Task<ServiceReply<bool>> DeleteSession( int sessionId );
    Task<ServiceReply<int>> AuthorizeSessionAndUserId( int sessionId, string sessionToken, UserDeviceInfoDto? deviceInfo, bool mustBeAdmin = false );
}