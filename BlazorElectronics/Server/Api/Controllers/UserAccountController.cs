using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Models.Sessions;
using BlazorElectronics.Server.Core.Models.Users;
using BlazorElectronics.Shared.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class UserAccountController : UserController
{
    public UserAccountController( ILogger<UserController> logger, IUserAccountService userAccountService, ISessionService sessionService ) : base( logger, userAccountService, sessionService ) { }

    [HttpPost( "register" )]
    public async Task<ActionResult<UserSessionResponse>> Register( [FromBody] UserRegisterRequest? request )
    {
        if ( !ValidateRegisterRequest( request ) )
            return BadRequest( BAD_REQUEST_MESSAGE );

        ServiceReply<UserLoginDto?> reply = await UserAccountService.Register( request!.Username, request.Email, request.Password, request.Phone );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "login" )]
    public async Task<ActionResult<UserSessionResponse>> Login( [FromBody] UserLoginRequest? request )
    {
        if ( !ValidateLoginRequest( request ) )
            return BadRequest( BAD_REQUEST_MESSAGE );

        ServiceReply<UserLoginDto?> loginReply = await UserAccountService.Login( request!.EmailOrUsername, request.Password );

        if ( !loginReply.Success )
            return GetReturnFromReply( loginReply );

        ServiceReply<SessionDto?> sessionReply = await SessionService.CreateSession( loginReply.Data!.UserId, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return GetReturnFromReply( sessionReply );

        var session = new UserSessionResponse
        {
            Email = loginReply.Data.Email,
            Username = loginReply.Data.Username,
            SessionId = sessionReply.Data!.Id,
            SessionToken = sessionReply.Data.Token,
            IsAdmin = loginReply.Data.IsAdmin
        };

        return Ok( session );
    }
    [HttpPost( "authorize" )]
    public async Task<ActionResult<bool>> AuthorizeSession( [FromBody] UserRequest request )
    {
        ServiceReply<bool> authorizeReply = await ValidateAndAuthorizeUser( request );
        return GetReturnFromReply( authorizeReply );
    }
    [HttpPost( "change-password" )]
    public async Task<ActionResult<bool>> ChangePassword( [FromBody] UserDataRequest<PasswordChangeRequest>? request )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId( request );

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        if ( !ValidateChangePasswordRequest( request!.Payload ) )
            return BadRequest( "Invalid password request!" );

        ServiceReply<bool> passwordReply = await UserAccountService.ChangePassword( userReply.Data, request.Payload!.Password );
        return GetReturnFromReply( passwordReply );
    }
    [HttpPost( "logout" )]
    public async Task<ActionResult<bool>> Logout( [FromBody] UserRequest? request )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId( request );

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );
        
        ServiceReply<bool> deleteReply = await SessionService.DeleteSession( request!.SessionId );
        return GetReturnFromReply( deleteReply );
    }

    static bool ValidateRegisterRequest( UserRegisterRequest? request )
    {
        if ( request is null )
            return false;
        
        bool validUsername = !string.IsNullOrWhiteSpace( request.Username );
        bool validEmail = !string.IsNullOrWhiteSpace( request.Email );
        bool validPassword = !string.IsNullOrWhiteSpace( request.Password );

        return validUsername && validEmail && validPassword;
    }
    static bool ValidateLoginRequest( UserLoginRequest? request )
    {
        if (request is null )
            return false;
        
        bool validEmailOrUsername = !string.IsNullOrWhiteSpace( request.EmailOrUsername );
        bool validPassword = !string.IsNullOrWhiteSpace( request.Password );

        return validEmailOrUsername && validPassword;
    }
    static bool ValidateChangePasswordRequest( PasswordChangeRequest? dto )
    {
        return dto is not null && !string.IsNullOrWhiteSpace( dto.Password );
    }
}