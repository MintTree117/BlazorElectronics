using BlazorElectronics.Server.Controllers;
using BlazorElectronics.Server.Dtos.Users;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Inbound.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Admin.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class _AdminController : UserController
{
    protected const string NO_DATA_MESSAGE = "No data found!";
    protected const string INVALID_ADMIN_TASK_MESSAGE = "Invalid Task Call!";
    protected const string ADMIN_TASK_FAIL_MESSAGE = "Task Failed To Execute!";
    protected const string ADMIN_TASK_INTERNAL_SERVER_ERROR = "An internal server error occured!";

    protected readonly ILogger<_AdminController> Logger;

    public _AdminController( ILogger<_AdminController> logger, IUserAccountService userAccountService, ISessionService sessionService )
        : base( userAccountService, sessionService )
    {
        Logger = logger;
    }

    [HttpPost( "authorize" )]
    public async Task<ActionResult<ApiReply<bool>>> AuthorizeAdmin( [FromBody] SessionApiRequest request )
    {
        ApiReply<bool> reply = await ValidateAdminRequest( request, GetRequestDeviceInfo() );

        return reply.Success
            ? Ok( new ApiReply<bool>( true ) )
            : Ok( new ApiReply<bool>( reply.Message ) );
    }
    
    protected async Task<ApiReply<bool>> ValidateAdminRequest( SessionApiRequest? request, UserDeviceInfoDto? deviceInfo )
    {
        if ( !ValidateSessionRequest( request ) )
            return new ApiReply<bool>( BAD_REQUEST_MESSAGE );
        
        ApiReply<int> sessionReply = await SessionService.AuthorizeSession( request!.SessionId, request.SessionToken, deviceInfo );

        if ( !sessionReply.Success )
            return new ApiReply<bool>( sessionReply.Message );
        
        ApiReply<int> adminIdReply = await UserAccountService.VerifyAdminId( sessionReply.Data );

        return adminIdReply.Success
            ? new ApiReply<bool>( true )
            : new ApiReply<bool>( adminIdReply.Message );
    }
    protected async Task<ApiReply<T?>> TryExecuteAdminQuery<T>( Delegate action, params object?[] args )
    {
        object? actionResult = action.DynamicInvoke( args );

        if ( actionResult is not Task<T?> actionTask )
            return new ApiReply<T?>( INVALID_ADMIN_TASK_MESSAGE );

        try
        {
            T? result = await actionTask;

            return result is not null
                ? new ApiReply<T?>( result )
                : new ApiReply<T?>( ADMIN_TASK_FAIL_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<T?>( ADMIN_TASK_INTERNAL_SERVER_ERROR );
        }
        catch ( IOException ioException )
        {
            Logger.LogError( ioException.Message, ioException );
            return new ApiReply<T?>( ADMIN_TASK_INTERNAL_SERVER_ERROR );
        }
        catch ( SqlException sqlException )
        {
            Logger.LogError( sqlException.Message, sqlException );
            return new ApiReply<T?>( ADMIN_TASK_INTERNAL_SERVER_ERROR );
        }
        catch ( Exception exception )
        {
            Logger.LogError( exception.Message, exception );
            return new ApiReply<T?>( ADMIN_TASK_INTERNAL_SERVER_ERROR );
        }
    }
    protected async Task<ApiReply<bool>> TryExecuteAdminTransaction( Delegate action, params object?[] args )
    {
        object? actionResult = action.DynamicInvoke( args );

        if ( actionResult is not Task<bool> actionTask )
            return new ApiReply<bool>( INVALID_ADMIN_TASK_MESSAGE );

        try
        {
            bool success = await actionTask;

            return success
                ? new ApiReply<bool>( true )
                : new ApiReply<bool>( ADMIN_TASK_FAIL_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<bool>( ADMIN_TASK_INTERNAL_SERVER_ERROR );
        }
        catch ( IOException ioException )
        {
            Logger.LogError( ioException.Message, ioException );
            return new ApiReply<bool>( ADMIN_TASK_INTERNAL_SERVER_ERROR );
        }
        catch ( SqlException sqlException )
        {
            Logger.LogError( sqlException.Message, sqlException );
            return new ApiReply<bool>( ADMIN_TASK_INTERNAL_SERVER_ERROR );
        }
        catch ( Exception exception )
        {
            Logger.LogError( exception.Message, exception );
            return new ApiReply<bool>( ADMIN_TASK_INTERNAL_SERVER_ERROR );
        }
    }
}