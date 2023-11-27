using System.Net;
using BlazorElectronics.Server.Dtos.Users;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

public class UserController : _Controller
{
    protected readonly IUserAccountService UserAccountService;
    protected readonly ISessionService SessionService;
    
    public UserController( ILogger<UserController> logger, IUserAccountService userAccountService, ISessionService sessionService )
        : base( logger )
    {
        UserAccountService = userAccountService;
        SessionService = sessionService;
    }
    
    protected UserDeviceInfoDto GetRequestDeviceInfo()
    {
        IPAddress? address = Request.HttpContext.Connection.RemoteIpAddress;
        var dto = new UserDeviceInfoDto( address?.ToString() );
        return dto;
    }
    protected async Task<HttpAuthorization> ValidateAndAuthorizeUser( UserRequest? request )
    {
        if ( !ValidateUserHttp( request ) )
            return new HttpAuthorization( 0, BadRequest( BAD_REQUEST_MESSAGE ) );

        ApiReply<int> authorize = await AuthorizeSessionRequest( request!.SessionId, request.SessionToken );

        return authorize.Success
            ? new HttpAuthorization( authorize.Data )
            : new HttpAuthorization( 0, Unauthorized( authorize.Message ) );
    }
    protected async Task<HttpAuthorization> ValidateAndAuthorizeUser<T>( UserDataRequest<T>? request ) where T : class
    {
        if ( !ValidateUserHttp( request ) )
            return new HttpAuthorization( 0, BadRequest( BAD_REQUEST_MESSAGE ) );

        ApiReply<int> authorize = await AuthorizeSessionRequest( request!.SessionId, request.SessionToken );

        return authorize.Success
            ? new HttpAuthorization( authorize.Data )
            : new HttpAuthorization( 0, Unauthorized( authorize.Message ) );
    }
    
    async Task<ApiReply<int>> AuthorizeSessionRequest( int sessionId, string sessionToken )
    {
        return await SessionService.AuthorizeSession( sessionId, sessionToken, GetRequestDeviceInfo() );
    }
    static bool ValidateUserHttp( UserRequest? request )
    {
        return request is not null && 
               ValidateRequestSession( request.SessionId, request.SessionToken );
    }
    static bool ValidateUserHttp<T>( UserDataRequest<T>? request ) where T : class
    {
        return request?.Payload is not null && 
               ValidateRequestSession( request.SessionId, request.SessionToken );
    }
    static bool ValidateRequestSession( int sessionId, string? sessionToken )
    {
        bool validId = sessionId >= 1;
        bool validToken = !string.IsNullOrWhiteSpace( sessionToken );

        return validId && validToken;
    }
}