using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Models.Users;
using BlazorElectronics.Shared.Sessions;
using BlazorElectronics.Shared.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public sealed class UserAccountController : _UserController
{
    public UserAccountController( ILogger<UserAccountController> logger, IUserAccountService userAccountService, ISessionService sessionService ) : base( logger, userAccountService, sessionService ) { }

    [HttpGet( "authorize" )]
    public async Task<ActionResult<bool>> AuthorizeSession()
    {
        ServiceReply<int> authorizeReply = await ValidateAndAuthorizeUserId();

        return authorizeReply.Success
            ? Ok( true )
            : GetReturnFromReply( authorizeReply );
    }
    [HttpPost( "login" )]
    public async Task<ActionResult<SessionDto?>> Login( [FromBody] LoginRequestDto request )
    {
        if ( !ValidateLoginRequest( request ) )
            return BadRequest( "Invalid Login Request!" );

        ServiceReply<UserLoginDto?> loginReply = await UserAccountService
            .Login( request.EmailOrUsername, request.Password );

        if ( !loginReply.Success || loginReply.Payload is null )
            return GetReturnFromReply( loginReply );

        ServiceReply<SessionDto?> sessionReply = await SessionService
            .CreateSession( loginReply.Payload, GetRequestDeviceInfo() );

        return GetReturnFromReply( sessionReply );
    }
    [HttpDelete( "logout" )]
    public async Task<ActionResult<bool>> Logout( int sessionId )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId();

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<bool> deleteReply = await SessionService.DeleteSession( sessionId );
        return GetReturnFromReply( deleteReply );
    }
    
    [HttpPut( "register" )]
    public async Task<ActionResult<bool>> Register( [FromBody] RegisterRequestDto request )
    {
        if ( !ValidateRegisterRequest( request ) )
            return BadRequest( "Invalid Register Request!" );

        ServiceReply<bool> reply = await UserAccountService
            .Register( request.Username, request.Email, request.Password, request.Phone );
        
        return GetReturnFromReply( reply );
    }
    [HttpPut( "activate" )]
    public async Task<ActionResult<bool>> Activate( [FromBody] string token )
    {
        ServiceReply<bool> activateReply = await UserAccountService.ActivateAccount( token );
        return GetReturnFromReply( activateReply );
    }
    [HttpGet( "resend-verification" )]
    public async Task<ActionResult<bool>> ResendVerification( [FromBody] string token )
    {
        ServiceReply<bool> resendReply = await UserAccountService.ResendVerificationEmail( token );
        return GetReturnFromReply( resendReply );
    }

    [HttpGet( "get-account-details" )]
    public async Task<ActionResult<bool>> GetDetails()
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId();

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<AccountDetailsDto?> detailsReply = await UserAccountService.GetAccountDetails( userReply.Payload );
        return GetReturnFromReply( detailsReply );
    }
    [HttpGet( "get-sessions" )]
    public async Task<ActionResult<List<SessionInfoDto>?>> GetSessions()
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId();

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<List<SessionInfoDto>?> detailsReply = await SessionService.GetUserSessions( userReply.Payload );
        return GetReturnFromReply( detailsReply );
    }
    
    [HttpPost( "update-account-details" )]
    public async Task<ActionResult<bool>> UpdateDetails( [FromBody] AccountDetailsDto dto )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId();

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<AccountDetailsDto?> detailsReply = await UserAccountService.UpdateAccountDetails( userReply.Payload, dto );
        return GetReturnFromReply( detailsReply );
    }
    [HttpPut( "update-password" )]
    public async Task<ActionResult<bool>> ChangePassword( [FromBody] PasswordRequestDto request )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId();

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        if ( !ValidateChangePasswordRequest( request ) )
            return BadRequest( "Invalid password request!" );

        ServiceReply<bool> passwordReply = await UserAccountService.UpdatePassword( userReply.Payload, request.Password );
        return GetReturnFromReply( passwordReply );
    }
    [HttpDelete( "delete-session" )]
    public async Task<ActionResult<List<SessionInfoDto>?>> DeleteSessions( [FromBody] int sessionId )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId();

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<bool> deleteReply = await SessionService.DeleteSession( sessionId );
        return GetReturnFromReply( deleteReply );
    }
    [HttpDelete( "delete-all-sessions" )]
    public async Task<ActionResult<List<SessionInfoDto>?>> DeleteAllSessions()
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId();

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<bool> deleteReply = await SessionService.DeleteAllSessions( userReply.Payload );
        return GetReturnFromReply( deleteReply );
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