using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Inbound.Users;
using BlazorElectronics.Shared.Outbound.Users;

namespace BlazorElectronics.Client.Services.Users;

public interface IUserServiceClient
{
    event Action<bool> AuthorizationChanged;
    
    Task<ServiceResponse<UserLoginResponse?>> Register( UserRegisterRequest request );
    Task<ServiceResponse<UserLoginResponse?>> Login( UserLoginRequest request );
    Task<ServiceResponse<bool>> Logout();
    Task<ServiceResponse<bool>> AuthorizeUser();
    Task<ServiceResponse<bool>> ChangePassword( UserChangePasswordRequest request );
}