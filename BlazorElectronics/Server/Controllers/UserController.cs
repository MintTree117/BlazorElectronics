using System.Net;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Inbound.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

public class UserController : ControllerBase
{
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

    protected async Task<ServiceResponse<ValidatedIdAndSession>> ValidateUserSession( SessionApiRequest request, string ipAddress )
    {
        ServiceResponse<int> idResponse = await UserAccountService.ValidateUserId( request.Username );

        if ( !idResponse.Success )
            return new ServiceResponse<ValidatedIdAndSession>( null, false, idResponse.Message ??= $"Failed to validate UserId for {request.Username}!" );

        ServiceResponse<string?> sessionResponse = await SessionService.GetExistingSession( idResponse.Data, request.SessionToken, ipAddress! );

        return !sessionResponse.Success 
            ? new ServiceResponse<ValidatedIdAndSession>( null, false, idResponse.Message ??= $"Failed to validate Session for {request.Username}!" ) 
            : new ServiceResponse<ValidatedIdAndSession>( new ValidatedIdAndSession( idResponse.Data, sessionResponse.Data! ), true, $"Successfully validated session for {request.Username}." );
    }

    protected bool ValidateApiRequest( SessionApiRequest? apiRequest, out string? ipAddress, out string message )
    {
        ipAddress = null;
        message = "Failed to validate request!";

        if ( apiRequest == null )
        {
            message = "Api request is null!";
            return false;
        }

        if ( GetRequestIpAddress( out ipAddress ) )
            return true;

        message = "Failed to validate ip address for request!";
        return false;

    }
    protected bool GetRequestIpAddress( out string? ipAddress )
    {
        ipAddress = null;

        IPAddress? address = Request.HttpContext.Connection.RemoteIpAddress;

        if ( address == null )
            return false;

        ipAddress = address.ToString();

        return !string.IsNullOrEmpty( ipAddress );
    }
}