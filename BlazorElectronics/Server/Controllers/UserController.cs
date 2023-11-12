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

    protected async Task<ApiReply<int>> ValidateUserSession( SessionApiRequest? request )
    {
        if ( request is null )
            return new ApiReply<int>( BAD_REQUEST_MESSAGE );
        
        ApiReply<int> idReply = await UserAccountService.ValidateUserId( request.Email );
        
        if ( !idReply.Success )
            return new ApiReply<int>( idReply.Message );
        
        ApiReply<bool> sessionReply = await SessionService.ValidateSession( idReply.Data, request.SessionToken, GetRequestDeviceInfo() );

        return sessionReply.Success
            ? new ApiReply<int>( idReply.Data )
            : new ApiReply<int>( sessionReply.Message );
    }
    
    protected UserDeviceInfoDto GetRequestDeviceInfo()
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