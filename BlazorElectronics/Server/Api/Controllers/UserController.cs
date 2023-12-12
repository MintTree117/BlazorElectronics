using System.Net;
using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Models.Users;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Users;

namespace BlazorElectronics.Server.Api.Controllers;

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

    protected async Task<ServiceReply<bool>> ValidateAndAuthorizeUser( UserRequest? request )
    {
        if ( !ValidateUserHttp( request ) )
            return new ServiceReply<bool>( ServiceErrorType.ValidationError );

        return await SessionService.AuthorizeSession( request!.SessionId, request.SessionToken, GetRequestDeviceInfo() );
    }
    protected async Task<ServiceReply<int>> ValidateAndAuthorizeUserId( UserRequest? request )
    {
        if ( !ValidateUserHttp( request ) )
            return new ServiceReply<int>( ServiceErrorType.ValidationError );

        return await SessionService.AuthorizeSessionId( request!.SessionId, request.SessionToken, GetRequestDeviceInfo() );
    }
    protected async Task<ServiceReply<int>> ValidateAndAuthorizeUserId<T>( UserDataRequest<T>? request ) where T : class
    {
        if ( !ValidateUserHttp( request ) )
            return new ServiceReply<int>( ServiceErrorType.ValidationError );

        return await SessionService.AuthorizeSessionId( request!.SessionId, request.SessionToken, GetRequestDeviceInfo() );
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