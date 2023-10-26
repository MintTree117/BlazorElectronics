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
    public async Task<ActionResult<ServiceResponse<UserLoginResponse>>> TestLogin()
    {
        if ( !GetRequestIpAddress( out string? address ) )
            return BadRequest( new ServiceResponse<UserLoginResponse?>( null, false, "Failed to get ip address for client!" ) );

        ServiceResponse<UserLogin?> userResponse = await UserAccountService.ValidateUserLogin( "martygrof3708@gmail.com", "Modernwarfare3?" );

        if ( !userResponse.Success )
            return BadRequest( userResponse );

        ServiceResponse<string?> sessionResponse = await SessionService.CreateNewSession( userResponse.Data!.UserId, address! );

        return sessionResponse.Success
            ? Ok( new ServiceResponse<UserLoginResponse>( new UserLoginResponse( userResponse.Data.Username, sessionResponse.Data! ), true, "Successfully logged user in." ) )
            : BadRequest( sessionResponse );
    }
    
    [HttpPost( "register" )]
    public async Task<ActionResult<ServiceResponse<UserLoginResponse>>> Register( [FromBody] UserRegisterRequest request )
    {
        if ( !GetRequestIpAddress( out string? address ) )
            return BadRequest( new ServiceResponse<UserLoginResponse?>( null, false, "Failed to get ip address for client!" ) );

        ServiceResponse<UserLogin?> registerResponse = await UserAccountService.RegisterUser( request );
        
        if ( !registerResponse.Success )
            return BadRequest( registerResponse );

        ServiceResponse<string?> sessionResponse = await SessionService.CreateNewSession( registerResponse.Data!.UserId, address! );

        return sessionResponse.Success
            ? Ok( new ServiceResponse<UserLoginResponse>( new UserLoginResponse( registerResponse.Data.Username, sessionResponse.Data! ), true, "Successfully logged user in." ) )
            : BadRequest( new ServiceResponse<UserLoginResponse?>( null, false, $"Registered user {request.Email}, but " + ( sessionResponse.Message ??= "failed to get session!" ) ) );
    }
    [HttpPost( "login" )]
    public async Task<ActionResult<ServiceResponse<UserLoginResponse>>> Login( UserLoginRequest request )
    {
        if ( !GetRequestIpAddress( out string? address ) )
            return BadRequest( new ServiceResponse<UserLoginResponse?>( null, false, "Failed to get ip address for client!" ) );

        ServiceResponse<UserLogin?> userResponse = await UserAccountService.ValidateUserLogin( request.Email, request.Password );

        if ( !userResponse.Success )
            return BadRequest( userResponse );
        
        ServiceResponse<string?> sessionResponse = await SessionService.CreateNewSession( userResponse.Data!.UserId, address! );

        return sessionResponse.Success
            ? Ok( new ServiceResponse<UserLoginResponse>( new UserLoginResponse( userResponse.Data.Username, sessionResponse.Data! ), true, "Successfully logged user in." ) )
            : BadRequest( sessionResponse );
    }
    [HttpPost( "authorize" )]
    public async Task<ActionResult<ServiceResponse<bool>>> ValidateUser( [FromBody] SessionApiRequest request )
    {
        if ( !ValidateApiRequest( request, out string? ipAddress, out string message ) )
            return BadRequest( new ServiceResponse<bool>( false, false, message ) );

        ServiceResponse<ValidatedIdAndSession> validateSessionResponse = await ValidateUserSession( request, ipAddress! );

        return validateSessionResponse.Success
            ? Ok( new ServiceResponse<bool>( true, true, $"Validated user {request.Username}." ) )
            : BadRequest( new ServiceResponse<bool>( true, true, $"Failed to validate user {request.Username}!" ) );
    }
    [HttpPost( "change-password" )]
    public async Task<ActionResult<ServiceResponse<bool>>> ChangePassword( UserChangePasswordRequest request )
    {
        if ( !ValidateApiRequest( request.ApiRequest, out string? ipAddress, out string message ) )
            return BadRequest( new ServiceResponse<bool>( false, false, message ) );

        ServiceResponse<ValidatedIdAndSession> validateSessionResponse = await ValidateUserSession( request.ApiRequest!, ipAddress! );

        if ( !validateSessionResponse.Success )
            return BadRequest( new ServiceResponse<bool>( false, false, validateSessionResponse.Message! ) );

        ServiceResponse<bool> passwordResponse = await UserAccountService.ChangePassword( validateSessionResponse.Data!.UserId, request.Password );

        return passwordResponse.Success
            ? Ok( passwordResponse )
            : BadRequest( passwordResponse );
    }
}