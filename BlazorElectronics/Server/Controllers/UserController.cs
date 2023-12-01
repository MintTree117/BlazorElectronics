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
    protected async Task<ServiceReply<int>> ValidateAndAuthorizeUser( UserRequest? request )
    {
        if ( !ValidateUserHttp( request ) )
            return new ServiceReply<int>( ServiceErrorType.ValidationError );

        ServiceReply<int> authorizeReply = await AuthorizeSessionRequest( request!.SessionId, request.SessionToken );

        return authorizeReply.Success
            ? new ServiceReply<int>( authorizeReply.Data )
            : new ServiceReply<int>( ServiceErrorType.Unauthorized );
    }
    protected async Task<ServiceReply<int>> ValidateAndAuthorizeUser<T>( UserDataRequest<T>? request ) where T : class
    {
        if ( !ValidateUserHttp( request ) )
            return new ServiceReply<int>( ServiceErrorType.ValidationError );

        ServiceReply<int> authorizeReply = await AuthorizeSessionRequest( request!.SessionId, request.SessionToken );

        return authorizeReply.Success
            ? new ServiceReply<int>( authorizeReply.Data )
            : new ServiceReply<int>( ServiceErrorType.Unauthorized );
    }
    
    async Task<ServiceReply<int>> AuthorizeSessionRequest( int sessionId, string sessionToken )
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