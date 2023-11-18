using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Inbound.Users;
using BlazorElectronics.Shared.Outbound.Users;

namespace BlazorElectronics.Client.Services.Users;

public interface IUserServiceClient
{
    event Action<UserSessionResponse?> SessionChanged;
    event Action<string>? SessionStorageError;
    
    Task<ApiReply<UserSessionResponse?>> Register( UserRegisterRequest request );
    Task<ApiReply<UserSessionResponse?>> Login( UserLoginRequest request );
    Task<ApiReply<UserSessionResponse?>> AuthorizeUser();
    Task<ApiReply<bool>> Logout();
    Task<ApiReply<bool>> ChangePassword( UserChangePasswordRequest request );
    Task<ApiReply<UserSessionResponse?>> TryGetLocalUserSession();
}