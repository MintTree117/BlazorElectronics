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
    public UserAccountController( IUserAccountService userAccountService, ISessionService sessionService ) : base( userAccountService, sessionService ) { }

    [HttpPost( "register" )]
    public async Task<ActionResult<ApiReply<UserLoginResponse>>> Register( [FromBody] UserRegisterRequest request )
    {
        if ( !ValidateRegisterRequest( request ) )
            return BadRequest( new ApiReply<UserLoginResponse>( BAD_REQUEST_MESSAGE ) );
        
        ApiReply<UserLoginResponse> loginReply = await GetLogin(
            await UserAccountService.Register( request.Username, request.Email, request.Password, request.Phone ), GetRequestDeviceInfo() );
        
        return Ok( loginReply );
    }
    [HttpPost( "login" )]
    public async Task<ActionResult<ApiReply<UserLoginResponse>>> Login( [FromBody] UserLoginRequest request )
    {
        if ( !ValidateLoginRequest( request ) )
            return BadRequest( new ApiReply<UserLoginResponse>( BAD_REQUEST_MESSAGE ) );
        
        ApiReply<UserLoginResponse> loginReply = await GetLogin( 
            await UserAccountService.Login( request.EmailOrUsername, request.Password ), GetRequestDeviceInfo() );
        
        return Ok( loginReply );
    }
    [HttpPost( "validate" )]
    public async Task<ActionResult<ApiReply<bool>>> ValidateSession( [FromBody] SessionApiRequest request )
    {
        if ( !ValidateSessionRequest( request ) )
            return BadRequest( new ApiReply<bool>( BAD_REQUEST_MESSAGE ) );
        
        ApiReply<int> sessionReply = await ValidateUserSession( request );
        return Ok( sessionReply );
    }
    [HttpPost( "change-password" )]
    public async Task<ActionResult<ApiReply<bool>>> ChangePassword( [FromBody] UserChangePasswordRequest request )
    {
        ApiReply<int> validateReply = await ValidateUserSession( request.ApiRequest );

        if ( !validateReply.Success )
            return BadRequest( new ApiReply<bool>( validateReply.Message ) );

        ApiReply<bool> passwordReply = await UserAccountService.ChangePassword( validateReply.Data, request.Password );

        return passwordReply.Success
            ? Ok( passwordReply )
            : Ok( new ApiReply<bool>( passwordReply.Message ) );
    }
    
    async Task<ApiReply<UserLoginResponse>> GetLogin( ApiReply<UserLoginDto?> loginReply, UserDeviceInfoDto? deviceInfo )
    {
        if ( !loginReply.Success || loginReply.Data is null )
            return new ApiReply<UserLoginResponse>( loginReply.Message );
        
        UserLoginDto login = loginReply.Data;
        ApiReply<string?> sessionReply = await SessionService.CreateSession( loginReply.Data.UserId, deviceInfo );

        if ( !sessionReply.Success || sessionReply.Data is null )
            return new ApiReply<UserLoginResponse>( sessionReply.Message );

        var response = new UserLoginResponse( login.Username, login.Email, sessionReply.Data, login.IsAdmin );
        return new ApiReply<UserLoginResponse>( response );
    }

    static bool ValidateRegisterRequest( UserRegisterRequest request )
    {
        bool validUsername = !string.IsNullOrWhiteSpace( request.Username );
        bool validEmail = !string.IsNullOrWhiteSpace( request.Email );
        bool validPassword = !string.IsNullOrWhiteSpace( request.Password );

        return validUsername && validEmail && validPassword;
    }
    static bool ValidateLoginRequest( UserLoginRequest request )
    {
        bool validEmailOrUsername = !string.IsNullOrWhiteSpace( request.EmailOrUsername );
        bool validPassword = !string.IsNullOrWhiteSpace( request.Password );

        return validEmailOrUsername && validPassword;
    }
}