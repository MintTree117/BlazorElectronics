using System.Net;
using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Models.Users;
using BlazorElectronics.Shared.Enums;
using Microsoft.Extensions.Primitives;

namespace BlazorElectronics.Server.Api.Controllers;

public class _UserController : _Controller
{
    const string HTTP_HEADER_SESSION_ID = "SessionId";
    const string HTTP_HEADER_AUTHORIZATION = "Authorization";
    
    protected readonly IUserAccountService UserAccountService;
    protected readonly ISessionService SessionService;

    public _UserController( ILogger<_UserController> logger, IUserAccountService userAccountService, ISessionService sessionService )
        : base( logger )
    {
        UserAccountService = userAccountService;
        SessionService = sessionService;
    }

    protected async Task<ServiceReply<int>> ValidateAndAuthorizeUserId( bool mustBeAdmin = false )
    {
        if ( !GetHeaderStrings( out int sessionId, out string sessionToken ) )
        {
            return new ServiceReply<int>( 
                ServiceErrorType.ValidationError, "Failed to validate session http header!" );   
        }

        return await SessionService.AuthorizeSessionWithUserId( 
            sessionId, sessionToken, GetRequestDeviceInfo(), mustBeAdmin );
    }
    protected UserDeviceInfoDto GetRequestDeviceInfo()
    {
        IPAddress? address = Request.HttpContext.Connection.RemoteIpAddress;
        var dto = new UserDeviceInfoDto( address?.ToString() );
        return dto;
    }
    bool GetHeaderStrings( out int sessionId, out string sessionToken )
    {
        sessionId = -1;
        sessionToken = string.Empty;

        if ( !HttpContext.Request.Headers.TryGetValue( HTTP_HEADER_SESSION_ID, out StringValues idHeader ) )
            return false;

        if ( !HttpContext.Request.Headers.TryGetValue( HTTP_HEADER_AUTHORIZATION, out StringValues authHeader ) )
            return false;

        var sessionIdString = idHeader.ToString();
        sessionToken = authHeader.ToString().Substring( "Bearer".Length ).Trim();

        return int.TryParse( sessionIdString, out sessionId ) && !string.IsNullOrWhiteSpace( sessionToken );
    }
}