using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Models.Sessions;
using BlazorElectronics.Server.Core.Models.Users;
using BlazorElectronics.Shared.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public sealed class UserAccountController : UserController
{
    public UserAccountController( ILogger<UserController> logger, IUserAccountService userAccountService, ISessionService sessionService ) : base( logger, userAccountService, sessionService ) { }

    [HttpPut( "register" )]
    public async Task<ActionResult<SessionReplyDto>> Register( [FromBody] RegisterRequestDto request )
    {
        if ( !ValidateRegisterRequest( request ) )
            return BadRequest( BAD_REQUEST_MESSAGE );

        ServiceReply<UserLoginDto?> reply = await UserAccountService.Register( request.Username, request.Email, request.Password, request.Phone );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "login" )]
    public async Task<ActionResult<SessionReplyDto>> Login( [FromBody] LoginRequestDto request )
    {
        if ( !ValidateLoginRequest( request ) )
            return BadRequest( BAD_REQUEST_MESSAGE );

        ServiceReply<UserLoginDto?> loginReply = await UserAccountService.Login( request.EmailOrUsername, request.Password );

        if ( !loginReply.Success || loginReply.Data is null )
            return GetReturnFromReply( loginReply );

        ServiceReply<SessionDto?> sessionReply = await SessionService.CreateSession( loginReply.Data.UserId, GetRequestDeviceInfo() );

        if ( !sessionReply.Success || sessionReply.Data is null )
            return GetReturnFromReply( sessionReply );

        var session = new SessionReplyDto
        {
            Email = loginReply.Data.Email,
            Username = loginReply.Data.Username,
            SessionId = sessionReply.Data.Id,
            SessionToken = sessionReply.Data.Token,
            IsAdmin = loginReply.Data.IsAdmin
        };

        return Ok( session );
    }
    [HttpPost( "authorize" )]
    public async Task<ActionResult<bool>> AuthorizeSession()
    {
        ServiceReply<int> authorizeReply = await ValidateAndAuthorizeUserId();

        return authorizeReply.Success
            ? true
            : GetReturnFromReply( authorizeReply );
    }
    [HttpPut( "change-password" )]
    public async Task<ActionResult<bool>> ChangePassword( [FromBody] PasswordRequestDto request )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId();

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        if ( !ValidateChangePasswordRequest( request ) )
            return BadRequest( "Invalid password request!" );

        ServiceReply<bool> passwordReply = await UserAccountService.ChangePassword( userReply.Data, request.Password );
        return GetReturnFromReply( passwordReply );
    }
    [HttpPut( "logout" )]
    public async Task<ActionResult<bool>> Logout()
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId();

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );
        
        ServiceReply<bool> deleteReply = await SessionService.DeleteSession( userReply.Data );
        return GetReturnFromReply( deleteReply );
    }
    [HttpPut( "activate" )]
    public async Task<ActionResult<bool>> Activate( [FromBody] string token )
    {
        ServiceReply<bool> activateReply = await UserAccountService.ActivateAccount( token );
        return GetReturnFromReply( activateReply );
    }

    static bool ValidateRegisterRequest( RegisterRequestDto request )
    {
        if ( request is null )
            return false;
        
        bool validUsername = !string.IsNullOrWhiteSpace( request.Username );
        bool validEmail = !string.IsNullOrWhiteSpace( request.Email );
        bool validPassword = !string.IsNullOrWhiteSpace( request.Password );

        return validUsername && validEmail && validPassword;
    }
    static bool ValidateLoginRequest( LoginRequestDto? request )
    {
        if (request is null )
            return false;
        
        bool validEmailOrUsername = !string.IsNullOrWhiteSpace( request.EmailOrUsername );
        bool validPassword = !string.IsNullOrWhiteSpace( request.Password );

        return validEmailOrUsername && validPassword;
    }
    static bool ValidateChangePasswordRequest( PasswordRequestDto dto )
    {
        return dto is not null && !string.IsNullOrWhiteSpace( dto.Password );
    }
}