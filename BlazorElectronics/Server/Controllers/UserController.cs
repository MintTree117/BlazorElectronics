using System.Net;
using BlazorElectronics.Server.Dtos.Users;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Inbound.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

public class UserController : ControllerBase
{
    protected const string BAD_REQUEST_MESSAGE = "Bad request!";
    protected readonly IUserAccountService UserAccountService;
    protected readonly ISessionService SessionService;
    
    public UserController( IUserAccountService userAccountService, ISessionService sessionService )
    {
        UserAccountService = userAccountService;
        SessionService = sessionService;
    }

    protected sealed class ValidatedIdAndSession
    {
        public ValidatedIdAndSession( int userId, string token )
        {
            UserId = userId;
            SessionToken = token;
        }
        
        public int UserId { get; set; }
        public string? SessionToken { get; set; }
    }

    protected async Task<ApiReply<ValidatedIdAndSession>> ValidateUserSession( SessionApiRequest? request )
    {
        if ( request is null )
            return new ApiReply<ValidatedIdAndSession>( BAD_REQUEST_MESSAGE );
        
        ApiReply<int> idResponse = await UserAccountService.ValidateUserId( request.Email );
        
        if ( !idResponse.Success )
            return new ApiReply<ValidatedIdAndSession>( idResponse.Message );
        
        ApiReply<string?> sessionResponse = await SessionService.GetExistingSession( idResponse.Data, request.SessionToken, GetRequestDeviceInfo() );
        
        return sessionResponse.Success 
            ? new ApiReply<ValidatedIdAndSession>( sessionResponse.Message ) 
            : new ApiReply<ValidatedIdAndSession>( new ValidatedIdAndSession( idResponse.Data, sessionResponse.Data! ) );
    }
    
    protected UserDeviceInfoDto? GetRequestDeviceInfo()
    {
        IPAddress? address = Request.HttpContext.Connection.RemoteIpAddress;
        var dto = new UserDeviceInfoDto( address?.ToString() );
        return dto;
    }
    protected static bool ValidateSessionRequest( SessionApiRequest? request )
    {
        if ( request is null )
            return false;
        
        bool validEmail = !string.IsNullOrWhiteSpace( request.Email );
        bool validToken = !string.IsNullOrWhiteSpace( request.SessionToken );

        return validEmail && validToken;
    }
}