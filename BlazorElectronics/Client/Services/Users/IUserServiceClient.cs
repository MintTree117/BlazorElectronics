using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Inbound.Users;
using BlazorElectronics.Shared.Outbound.Users;

namespace BlazorElectronics.Client.Services.Users;

public interface IUserServiceClient
{
    event Action<bool> AuthorizationChanged;
    
    Task<ApiReply<UserLoginResponse?>> Register( UserRegisterRequest request );
    Task<ApiReply<UserLoginResponse?>> Login( UserLoginRequest request );
    Task<ApiReply<bool>> Logout();
    Task<ApiReply<bool>> AuthorizeUser();
    Task<ApiReply<bool>> ChangePassword( UserChangePasswordRequest request );
}