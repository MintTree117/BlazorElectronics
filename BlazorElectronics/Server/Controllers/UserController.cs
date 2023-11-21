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
    
    public UserController( IUserAccountService userAccountService, ISessionService sessionService )
    {
        UserAccountService = userAccountService;
        SessionService = sessionService;
    }

    protected async Task<ApiReply<int>> AuthorizeUserSession( UserApiRequest? request )
    {
        if ( request is null )
            return new ApiReply<int>( BAD_REQUEST_MESSAGE );
        
        ApiReply<int> sessionReply = await SessionService.AuthorizeSession( request.SessionId, request.SessionToken, GetRequestDeviceInfo() );
        
        return sessionReply.Success
            ? new ApiReply<int>( sessionReply.Data, true, "" )
            : new ApiReply<int>( -1, false, sessionReply.Message );
    }
    
    protected UserDeviceInfoDto GetRequestDeviceInfo()
    {
        IPAddress? address = Request.HttpContext.Connection.RemoteIpAddress;
        var dto = new UserDeviceInfoDto( address?.ToString() );
        return dto;
    }
    protected static bool ValidateSessionRequest( UserApiRequest? request )
    {
        if ( request is null )
            return false;

        bool validId = request.SessionId >= 0;
        bool validToken = !string.IsNullOrWhiteSpace( request.SessionToken );

        return validId && validToken;
    }
}