using BlazorElectronics.Client.Models;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Sessions;
using BlazorElectronics.Shared.Users;

namespace BlazorElectronics.Client.Services.Users;

public interface IUserServiceClient
{
    delegate Task AsyncEventHandler( object sender, SessionMeta? args );
    public event AsyncEventHandler SessionChanged;

    Task<SessionMeta?> GetSessionMeta();
    
    Task<ServiceReply<bool>> AuthorizeUser();
    Task<ServiceReply<SessionDto?>> Login( LoginRequestDto loginRequest );
    Task<ServiceReply<bool>> Logout();
    
    Task<ServiceReply<bool>> Register( RegisterRequestDto requestDto );
    Task<ServiceReply<bool>> ActivateAccount( string token );
    Task<ServiceReply<bool>> ResendVerification();
    
    Task<ServiceReply<AccountDetailsDto?>> GetAccountDetails();
    Task<ServiceReply<List<SessionInfoDto>?>> GetUserSessions();

    Task<ServiceReply<AccountDetailsDto?>> UpdateAccountDetails( AccountDetailsDto dto );
    Task<ServiceReply<bool>> UpdatePassword( PasswordRequestDto requestDto );
    Task<ServiceReply<bool>> DeleteSession( int sessionId );
    Task<ServiceReply<bool>> DeleteAllSessions();
}