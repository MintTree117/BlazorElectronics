using BlazorElectronics.Client.Models;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Users;

namespace BlazorElectronics.Client.Services.Users;

public interface IUserServiceClient
{
    event Action<SessionMeta?> SessionChanged;
    
    Task<ServiceReply<bool>> Register( RegisterRequestDto requestDto );
    Task<ServiceReply<SessionReplyDto?>> Login( LoginRequestDto requestDto );
    Task<ServiceReply<bool>> AuthorizeUser();
    Task<ServiceReply<bool>> Logout();
    Task<ServiceReply<bool>> ChangePassword( PasswordRequestDto requestDto );
    Task<SessionMeta?> GetSessionMeta();
}