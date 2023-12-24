using System.Net;
using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Models.Users;
using BlazorElectronics.Shared.Enums;
using Microsoft.Extensions.Primitives;

namespace BlazorElectronics.Server.Api.Controllers;

public class UserController : _Controller
{
    const string ID_HEADER = "SessionId";
    const string AUTH_HEADER = "Authorization";
    
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

    protected async Task<ServiceReply<int>> ValidateAndAuthorizeUserId( bool mustBeAdmin = false )
    {
        if ( !GetHeaderStrings( out int sessionId, out string sessionToken ) )
            return new ServiceReply<int>( ServiceErrorType.ValidationError, "Failed to validate http header!" );

        return await SessionService.AuthorizeSessionAndUserId( sessionId, sessionToken, GetRequestDeviceInfo(), mustBeAdmin );
    }

    bool GetHeaderStrings( out int sessionId, out string sessionToken )
    {
        sessionId = -1;
        sessionToken = string.Empty;

        if ( !HttpContext.Request.Headers.TryGetValue( ID_HEADER, out StringValues idHeader ) )
            return false;

        if ( !HttpContext.Request.Headers.TryGetValue( AUTH_HEADER, out StringValues authHeader ) )
            return false;

        var sessionIdString = idHeader.ToString();
        sessionToken = authHeader.ToString().Substring( "Bearer".Length ).Trim();

        return int.TryParse( sessionIdString, out sessionId ) && !string.IsNullOrWhiteSpace( sessionToken );
    }
}