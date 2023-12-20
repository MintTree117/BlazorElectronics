using BlazorElectronics.Client.Models;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Users;

namespace BlazorElectronics.Client.Services.Users;

public interface IUserServiceClient
{
    delegate Task AsyncEventHandler( object sender, SessionMeta? args );
    public event AsyncEventHandler SessionChanged;
    
    Task<ServiceReply<bool>> Register( RegisterRequestDto requestDto );
    Task<ServiceReply<bool>> ActivateAccount( string token );
    Task<ServiceReply<SessionReplyDto?>> Login( LoginRequestDto requestDto );
    Task<ServiceReply<bool>> AuthorizeUser();
    Task<ServiceReply<bool>> Logout();
    Task<ServiceReply<bool>> ChangePassword( PasswordRequestDto requestDto );
    Task<SessionMeta?> GetSessionMeta();
}