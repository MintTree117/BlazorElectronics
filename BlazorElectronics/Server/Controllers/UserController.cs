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

    protected async Task<ApiReply<ValidatedUserApiRequest<T?>>> TryValidateUserRequest<T>( UserApiRequest? request ) where T : class
    {
        if ( !TryValidateUserApiRequestData( request, out T? data ) )
            return new ApiReply<ValidatedUserApiRequest<T?>>( "Failed to validate http request!" );

        ApiReply<int> sessionReply = await SessionService.AuthorizeSession( request!.SessionId, request.SessionToken, GetRequestDeviceInfo() );

        if ( !sessionReply.Success )
            return new ApiReply<ValidatedUserApiRequest<T?>>( sessionReply.Message );

        var validatedRequest = new ValidatedUserApiRequest<T?>( sessionReply.Data, data );
        return new ApiReply<ValidatedUserApiRequest<T?>>( validatedRequest );
    }

    protected UserDeviceInfoDto GetRequestDeviceInfo()
    {
        IPAddress? address = Request.HttpContext.Connection.RemoteIpAddress;
        var dto = new UserDeviceInfoDto( address?.ToString() );
        return dto;
    }
    protected static bool TryValidateUserApiRequestData<T>( UserApiRequest? request, out T? data ) where T : class
    {
        data = default;
        
        if ( request is null )
            return false;

        bool validId = request.SessionId >= 1;
        bool validToken = !string.IsNullOrWhiteSpace( request.SessionToken );

        if ( !validId && validToken )
            return false;

        data = request.Data as T;
        
        return validId && validToken;
    }
}