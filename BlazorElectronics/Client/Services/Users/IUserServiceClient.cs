using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Users;

namespace BlazorElectronics.Client.Services.Users;

public interface IUserServiceClient
{
    event Action<UserSessionResponse?> SessionChanged;

    Task<ServiceReply<UserSessionResponse?>> Register( UserRegisterRequest request );
    Task<ServiceReply<UserSessionResponse?>> Login( UserLoginRequest request );
    Task<ServiceReply<UserSessionResponse?>> AuthorizeUser();
    Task<ServiceReply<bool>> Logout();
    Task<ServiceReply<bool>> ChangePassword( PasswordChangeRequest request );
    Task<ServiceReply<UserSessionResponse?>> TryGetLocalUserSession();
}