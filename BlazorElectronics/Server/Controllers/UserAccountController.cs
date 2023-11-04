using BlazorElectronics.Server.Models.Users;
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
    public UserAccountController( IUserAccountService userAccountService, ISessionService sessionService ) : base( userAccountService, sessionService ) { }

    [HttpGet( "test-login" )]
    public async Task<ActionResult<Reply<UserLoginResponse>>> TestLogin()
    {
        if ( !GetRequestIpAddress( out string? address ) )
            return BadRequest( new Reply<UserLoginResponse?>( null, false, "Failed to get ip address for client!" ) );

        Reply<UserLogin?> userResponse = await UserAccountService.ValidateUserLogin( "martygrof3708@gmail.com", "Modernwarfare3?" );

        if ( !userResponse.Success )
            return BadRequest( userResponse );

        Reply<string?> sessionResponse = await SessionService.CreateNewSession( userResponse.Data!.UserId, address! );

        return sessionResponse.Success
            ? Ok( new Reply<UserLoginResponse>( new UserLoginResponse( userResponse.Data.Username, sessionResponse.Data! ), true, "Successfully logged user in." ) )
            : BadRequest( sessionResponse );
    }
    
    [HttpPost( "register" )]
    public async Task<ActionResult<Reply<UserLoginResponse>>> Register( [FromBody] UserRegisterRequest request )
    {
        if ( !GetRequestIpAddress( out string? address ) )
            return BadRequest( new Reply<UserLoginResponse?>( null, false, "Failed to get ip address for client!" ) );

        Reply<UserLogin?> registerResponse = await UserAccountService.RegisterUser( request );
        
        if ( !registerResponse.Success )
            return BadRequest( registerResponse );

        Reply<string?> sessionResponse = await SessionService.CreateNewSession( registerResponse.Data!.UserId, address! );

        return sessionResponse.Success
            ? Ok( new Reply<UserLoginResponse>( new UserLoginResponse( registerResponse.Data.Username, sessionResponse.Data! ), true, "Successfully logged user in." ) )
            : BadRequest( new Reply<UserLoginResponse?>( null, false, $"Registered user {request.Email}, but " + ( sessionResponse.Message ??= "failed to get session!" ) ) );
    }
    [HttpPost( "login" )]
    public async Task<ActionResult<Reply<UserLoginResponse>>> Login( UserLoginRequest request )
    {
        if ( !GetRequestIpAddress( out string? address ) )
            return BadRequest( new Reply<UserLoginResponse?>( null, false, "Failed to get ip address for client!" ) );

        Reply<UserLogin?> userResponse = await UserAccountService.ValidateUserLogin( request.Email, request.Password );

        if ( !userResponse.Success )
            return BadRequest( userResponse );
        
        Reply<string?> sessionResponse = await SessionService.CreateNewSession( userResponse.Data!.UserId, address! );

        return sessionResponse.Success
            ? Ok( new Reply<UserLoginResponse>( new UserLoginResponse( userResponse.Data.Username, sessionResponse.Data! ), true, "Successfully logged user in." ) )
            : BadRequest( sessionResponse );
    }
    [HttpPost( "authorize" )]
    public async Task<ActionResult<Reply<bool>>> ValidateUser( [FromBody] SessionApiRequest request )
    {
        if ( !ValidateApiRequest( request, out string? ipAddress, out string message ) )
            return BadRequest( new Reply<bool>( false, false, message ) );

        Reply<ValidatedIdAndSession> validateSessionResponse = await ValidateUserSession( request, ipAddress! );

        return validateSessionResponse.Success
            ? Ok( new Reply<bool>( true, true, $"Validated user {request.Username}." ) )
            : BadRequest( new Reply<bool>( true, true, $"Failed to validate user {request.Username}!" ) );
    }
    [HttpPost( "change-password" )]
    public async Task<ActionResult<Reply<bool>>> ChangePassword( UserChangePasswordRequest request )
    {
        if ( !ValidateApiRequest( request.ApiRequest, out string? ipAddress, out string message ) )
            return BadRequest( new Reply<bool>( false, false, message ) );

        Reply<ValidatedIdAndSession> validateSessionResponse = await ValidateUserSession( request.ApiRequest!, ipAddress! );

        if ( !validateSessionResponse.Success )
            return BadRequest( new Reply<bool>( false, false, validateSessionResponse.Message! ) );

        Reply<bool> passwordResponse = await UserAccountService.ChangePassword( validateSessionResponse.Data!.UserId, request.Password );

        return passwordResponse.Success
            ? Ok( passwordResponse )
            : BadRequest( passwordResponse );
    }
}