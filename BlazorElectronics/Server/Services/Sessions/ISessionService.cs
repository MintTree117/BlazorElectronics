using BlazorElectronics.Server.Dtos.Users;

namespace BlazorElectronics.Server.Services.Sessions;

public interface ISessionService
{
    Task<ApiReply<string?>> CreateSession( int userId, UserDeviceInfoDto? deviceInfo );
    Task<ApiReply<string?>> GetExistingSession( int userId, string sessionToken, UserDeviceInfoDto? deviceInfo );
}