using BlazorElectronics.Server.Core.Models.Users;
using BlazorElectronics.Shared.Sessions;


namespace BlazorElectronics.Server.Api.Interfaces;

public interface ISessionService
{
    Task<ServiceReply<List<SessionInfoDto>?>> GetUserSessions( int userId );
    Task<ServiceReply<SessionDto?>> CreateSession( UserLoginDto login, UserDeviceInfoDto? deviceInfo );
    Task<ServiceReply<bool>> DeleteSession( int sessionId );
    Task<ServiceReply<bool>> DeleteAllSessions( int userId );
    Task<ServiceReply<int>> AuthorizeSessionAndUserId( int sessionId, string sessionToken, UserDeviceInfoDto? deviceInfo, bool mustBeAdmin = false );
}