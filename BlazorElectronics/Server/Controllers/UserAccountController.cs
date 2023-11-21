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
    public UserAccountController( IUserAccountService userAccountService, ISessionService sessionService ) : base( userAccountService, sessionService ) { }

    [HttpPost( "register" )]
    public async Task<ActionResult<ApiReply<UserSessionResponse>>> Register( [FromBody] UserApiRequest? apiRequest )
    {
        if ( !ValidateRegisterRequest( apiRequest, out UserRegisterRequest? dto ) )
            return BadRequest( BAD_REQUEST_MESSAGE );
        
        return await GetLogin(
            await UserAccountService.Register( dto!.Username, dto.Email, dto.Password, dto.Phone ), GetRequestDeviceInfo() );
    }
    [HttpPost( "login" )]
    public async Task<ActionResult<ApiReply<UserSessionResponse>>> Login( [FromBody] UserApiRequest? apiRequest )
    {
        if ( !ValidateLoginRequest( apiRequest, out UserLoginRequest? dto ) )
            return BadRequest( BAD_REQUEST_MESSAGE );
        
        return await GetLogin( 
            await UserAccountService.Login( dto!.EmailOrUsername, dto.Password ), GetRequestDeviceInfo() );
    }
    [HttpPost( "authorize" )]
    public async Task<ActionResult<ApiReply<bool>>> AuthorizeSession( [FromBody] UserApiRequest? apiRequest )
    {
        ApiReply<ValidatedUserApiRequest<object?>> reply = await TryValidateUserRequest<object>( apiRequest );

        return reply.Success
            ? Ok( new ApiReply<bool>( true ) )
            : BadRequest( reply.Message );
    }
    [HttpPost( "change-password" )]
    public async Task<ActionResult<ApiReply<bool>>> ChangePassword( [FromBody] UserApiRequest? apiRequest )
    {
        ApiReply<ValidatedUserApiRequest<UserChangePasswordRequest?>> validateReply = await TryValidateUserRequest<UserChangePasswordRequest>( apiRequest );

        if ( !validateReply.Success || !ValidateChangePasswordRequest( validateReply.Data?.Dto ) )
            return BadRequest( validateReply.Message );

        return await UserAccountService.ChangePassword( validateReply.Data!.UserId, validateReply.Data.Dto!.Password );
    }
    [HttpPost( "logout" )]
    public async Task<ActionResult<ApiReply<bool>>> Logout( [FromBody] UserApiRequest? apiRequest )
    {
        ApiReply<ValidatedUserApiRequest<object?>> validateReply = await TryValidateUserRequest<object>( apiRequest );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        return await SessionService.DeleteSession( apiRequest!.SessionId );
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

    static bool ValidateRegisterRequest( UserApiRequest? apiRequest, out UserRegisterRequest? dto )
    {
        dto = null;
        
        if ( !TryValidateUserApiRequestData( apiRequest, out dto ) )
            return false;
        
        if ( dto is null )
            return false;
        
        bool validUsername = !string.IsNullOrWhiteSpace( dto.Username );
        bool validEmail = !string.IsNullOrWhiteSpace( dto.Email );
        bool validPassword = !string.IsNullOrWhiteSpace( dto.Password );

        return validUsername && validEmail && validPassword;
    }
    static bool ValidateLoginRequest( UserApiRequest? apiRequest, out UserLoginRequest? dto )
    {
        if ( !TryValidateUserApiRequestData( apiRequest, out dto ) )
            return false;
        
        if ( dto is null )
            return false;
        
        bool validEmailOrUsername = !string.IsNullOrWhiteSpace( dto.EmailOrUsername );
        bool validPassword = !string.IsNullOrWhiteSpace( dto.Password );

        return validEmailOrUsername && validPassword;
    }
    static bool ValidateChangePasswordRequest( UserChangePasswordRequest? dto )
    {
        return dto is not null && !string.IsNullOrWhiteSpace( dto.Password );
    }
}