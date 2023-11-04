using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Inbound.Users;
using BlazorElectronics.Shared.Outbound.Users;

namespace BlazorElectronics.Client.Services.Users;

public interface IUserServiceClient
{
    event Action<bool> AuthorizationChanged;
    
    Task<Reply<UserLoginResponse?>> Register( UserRegisterRequest request );
    Task<Reply<UserLoginResponse?>> Login( UserLoginRequest request );
    Task<Reply<bool>> Logout();
    Task<Reply<bool>> AuthorizeUser();
    Task<Reply<bool>> ChangePassword( UserChangePasswordRequest request );
}