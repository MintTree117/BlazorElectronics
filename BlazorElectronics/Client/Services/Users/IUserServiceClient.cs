using BlazorElectronics.Client.Models;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Users;

namespace BlazorElectronics.Client.Services.Users;

public interface IUserServiceClient
{
    event Action<SessionMeta?> SessionChanged;
    
    Task<ServiceReply<bool>> Register( UserRegisterRequest request );
    Task<ServiceReply<UserSessionResponse?>> Login( UserLoginRequest request );
    Task<ServiceReply<bool>> AuthorizeUser();
    Task<ServiceReply<bool>> Logout();
    Task<ServiceReply<bool>> ChangePassword( PasswordChangeRequest request );
    Task<SessionMeta?> GetSessionMeta();
}