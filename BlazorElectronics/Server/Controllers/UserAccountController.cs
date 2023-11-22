using BlazorElectronics.Server.Dtos.Sessions;
using BlazorElectronics.Server.Dtos.Users;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Inbound.Users;
using BlazorElectronics.Shared.Outbound.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class UserAccountController : UserController
{
    public UserAccountController( ILogger<UserController> logger, IUserAccountService userAccountService, ISessionService sessionService ) : base( logger, userAccountService, sessionService ) { }

    [HttpPost( "register" )]
    public async Task<ActionResult<ApiReply<UserSessionResponse>>> Register( [FromBody] UserRegisterRequest? request )
    {
        if ( !ValidateRegisterRequest( request ) )
            return BadRequest( BAD_REQUEST_MESSAGE );
        
        return await GetLogin(
            await UserAccountService.Register( request!.Username, request.Email, request.Password, request.Phone ), GetRequestDeviceInfo() );
    }
    [HttpPost( "login" )]
    public async Task<ActionResult<ApiReply<UserSessionResponse>>> Login( [FromBody] UserLoginRequest? request )
    {
        if ( !ValidateLoginRequest( request ) )
            return BadRequest( BAD_REQUEST_MESSAGE );
        
        return await GetLogin( 
            await UserAccountService.Login( request!.EmailOrUsername, request.Password ), GetRequestDeviceInfo() );
    }
    [HttpPost( "authorize" )]
    public async Task<ActionResult<ApiReply<bool>>> AuthorizeSession( [FromBody] UserRequest request )
    {
        ApiReply<int> reply = await AuthorizeSessionRequest( request );

        return reply.Success
            ? Ok( new ApiReply<bool>( true ) )
            : BadRequest( reply.Message );
    }
    [HttpPost( "change-password" )]
    public async Task<ActionResult<ApiReply<bool>>> ChangePassword( [FromBody] UserDataRequest<PasswordChangeRequest>? request )
    {
        ApiReply<int> validateReply = await AuthorizeSessionRequest( request );
        
        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        if ( !ValidateChangePasswordRequest( request!.Payload ) )
            return BadRequest( "Invalid password request!" );

        return await UserAccountService.ChangePassword( validateReply.Data, request.Payload!.Password );
    }
    [HttpPost( "logout" )]
    public async Task<ActionResult<ApiReply<bool>>> Logout( [FromBody] UserRequest? request )
    {
        ApiReply<int> validateReply = await AuthorizeSessionRequest( request );
        
        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );
        
        return await SessionService.DeleteSession( request!.SessionId );
    }
    
    async Task<ApiReply<UserSessionResponse>> GetLogin( ApiReply<UserLoginDto?> loginReply, UserDeviceInfoDto? deviceInfo )
    {
        if ( !loginReply.Success || loginReply.Data is null )
            return new ApiReply<UserSessionResponse>( loginReply.Message );
        
        UserLoginDto login = loginReply.Data;
        ApiReply<SessionDto?> sessionReply = await SessionService.CreateSession( loginReply.Data.UserId, deviceInfo );

        if ( !sessionReply.Success || sessionReply.Data is null )
            return new ApiReply<UserSessionResponse>( sessionReply.Message );
        
        var response = new UserSessionResponse
        {
            Email = login.Email,
            Username = login.Username,
            SessionId = sessionReply.Data.Id,
            SessionToken = sessionReply.Data.Token,
            IsAdmin = login.IsAdmin
        };
        
        return new ApiReply<UserSessionResponse>( response );
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