using System.Net;
using BlazorElectronics.Server.Dtos.Users;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Inbound.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

public class UserController : ControllerBase
{
    protected const string BAD_REQUEST_MESSAGE = "Bad Request!";
    protected readonly IUserAccountService UserAccountService;
    protected readonly ISessionService SessionService;

    protected readonly ILogger<UserController> Logger;

    public UserController( ILogger<UserController> logger, IUserAccountService userAccountService, ISessionService sessionService )
    {
        Logger = logger;
        UserAccountService = userAccountService;
        SessionService = sessionService;
    }

    protected async Task<ApiReply<int>> AuthorizeSessionRequest( UserRequest? request )
    {
        if ( !ValidateUserHttp( request ) )
            return new ApiReply<int>( "Failed to validate http request!" );

        return await SessionService.AuthorizeSession( request!.SessionId, request.SessionToken, GetRequestDeviceInfo() );
    }
    protected async Task<ApiReply<int>> AuthorizeSessionRequest<T>( UserDataRequest<T>? request ) where T : class
    {
        if ( !ValidateUserHttp( request ) )
        {
            return new ApiReply<int>( "Failed to validate http request!" );   
        }

        return await SessionService.AuthorizeSession( request!.SessionId, request.SessionToken, GetRequestDeviceInfo() );
    }
    
    protected UserDeviceInfoDto GetRequestDeviceInfo()
    {
        IPAddress? address = Request.HttpContext.Connection.RemoteIpAddress;
        var dto = new UserDeviceInfoDto( address?.ToString() );
        return dto;
    }
    static bool ValidateUserHttp( UserRequest? request )
    {
        return request is not null && ValidateRequestSession( request.SessionId, request.SessionToken );
    }
    static bool ValidateUserHttp<T>( UserDataRequest<T>? request ) where T : class
    {
        if ( request?.Payload is null )
            return false;

        if ( !ValidateRequestSession( request.SessionId, request.SessionToken ) )
            return false;
        
        return true;
    }
    static bool ValidateRequestSession( int sessionId, string? sessionToken )
    {
        bool validId = sessionId >= 1;
        bool validToken = !string.IsNullOrWhiteSpace( sessionToken );

        return validId && validToken;
    }
}