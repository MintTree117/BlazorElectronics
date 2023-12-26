using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Models.Users;
using BlazorElectronics.Shared.Sessions;
using BlazorElectronics.Shared.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public sealed class UserAccountController : UserController
{
    public UserAccountController( ILogger<UserController> logger, IUserAccountService userAccountService, ISessionService sessionService ) : base( logger, userAccountService, sessionService ) { }

    [HttpPut( "register" )]
    public async Task<ActionResult<SessionDto?>> Register( [FromBody] RegisterRequestDto request )
    {
        if ( !ValidateRegisterRequest( request ) )
            return BadRequest( BAD_REQUEST_MESSAGE );

        ServiceReply<UserLoginDto?> reply = await UserAccountService.Register( request.Username, request.Email, request.Password, request.Phone );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "login" )]
    public async Task<ActionResult<SessionDto?>> Login( [FromBody] LoginRequestDto request )
    {
        if ( !ValidateLoginRequest( request ) )
            return BadRequest( BAD_REQUEST_MESSAGE );

        ServiceReply<UserLoginDto?> loginReply = await UserAccountService.Login( request.EmailOrUsername, request.Password );

        if ( !loginReply.Success || loginReply.Data is null )
            return GetReturnFromReply( loginReply );

        ServiceReply<SessionDto?> sessionReply = await SessionService.CreateSession( loginReply.Data, GetRequestDeviceInfo() );

        return GetReturnFromReply( sessionReply );
    }
    [HttpGet( "authorize" )]
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
    [HttpDelete( "logout" )]
    public async Task<ActionResult<bool>> Logout( int sessionId )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId();

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );
        
        ServiceReply<bool> deleteReply = await SessionService.DeleteSession( sessionId );
        return GetReturnFromReply( deleteReply );
    }
    [HttpGet( "resend-verification" )]
    public async Task<ActionResult<bool>> ResendVerification( [FromBody] string token )
    {
        ServiceReply<bool> resendReply = await UserAccountService.ResendVerificationEmail( token );
        return GetReturnFromReply( resendReply );
    }
    [HttpPut( "activate" )]
    public async Task<ActionResult<bool>> Activate( [FromBody] string token )
    {
        ServiceReply<bool> activateReply = await UserAccountService.ActivateAccount( token );
        return GetReturnFromReply( activateReply );
    }
    [HttpGet( "account-details" )]
    public async Task<ActionResult<bool>> GetDetails()
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId();

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<AccountDetailsDto?> detailsReply = await UserAccountService.GetAccountDetails( userReply.Data );
        return GetReturnFromReply( detailsReply );
    }
    [HttpPost( "update-account-details" )]
    public async Task<ActionResult<bool>> UpdateDetails( AccountDetailsDto dto )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId();

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<AccountDetailsDto?> detailsReply = await UserAccountService.UpdateAccountDetails( userReply.Data, dto );
        return GetReturnFromReply( detailsReply );
    }
    [HttpGet( "get-sessions" )]
    public async Task<ActionResult<List<SessionInfoDto>?>> GetSessions()
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId();

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<List<SessionInfoDto>?> detailsReply = await SessionService.GetUserSessions( userReply.Data );
        return GetReturnFromReply( detailsReply );
    }
    [HttpDelete( "delete-all-sessions" )]
    public async Task<ActionResult<List<SessionInfoDto>?>> DeleteAllSessions()
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId();

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<bool> detailsReply = await SessionService.DeleteAllSessions( userReply.Data );
        return GetReturnFromReply( detailsReply );
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