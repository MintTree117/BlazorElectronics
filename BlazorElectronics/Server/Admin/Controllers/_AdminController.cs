using BlazorElectronics.Server.Controllers;
using BlazorElectronics.Server.Dtos.Users;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Inbound.Users;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Admin.Controllers;

public class _AdminController : UserController
{
    const string INVALID_ADMIN_TASK_MESSAGE = "Invalid Task Call!";
    const string ADMIN_TASK_FAIL_MESSAGE = "Task Failed To Execute!";
    const string ADMIN_TASK_INTERNAL_SERVER_ERROR = "An internal server error occured!";

    readonly ILogger _logger;
    
    public _AdminController( ILogger logger, IUserAccountService userAccountService, ISessionService sessionService )
        : base( userAccountService, sessionService )
    {
        _logger = logger;
    }

    protected async Task<ApiReply<bool>> ValidateAdminRequest( SessionApiRequest? request, UserDeviceInfoDto? deviceInfo )
    {
        if ( !ValidateSessionRequest( request ) )
            return new ApiReply<bool>( BAD_REQUEST_MESSAGE );
        
        ApiReply<int> sessionReply = await SessionService.ValidateSession( request!.SessionId, request.SessionToken, deviceInfo );

        if ( !sessionReply.Success )
            return new ApiReply<bool>( sessionReply.Message );
        
        ApiReply<int> adminIdReply = await UserAccountService.VerifyAdminId( sessionReply.Data );

        return adminIdReply.Success
            ? new ApiReply<bool>( true )
            : new ApiReply<bool>( adminIdReply.Message );
    }
    protected async Task<ApiReply<bool>> TryExecuteAdminAction( Delegate action, params object?[] args )
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
        catch ( IOException ioException )
        {
            _logger.LogError( ioException.Message, ioException );
            return new ApiReply<bool>( ADMIN_TASK_INTERNAL_SERVER_ERROR );
        }
        catch ( SqlException sqlException )
        {
            _logger.LogError( sqlException.Message, sqlException );
            return new ApiReply<bool>( ADMIN_TASK_INTERNAL_SERVER_ERROR );
        }
        catch ( Exception exception )
        {
            _logger.LogError( exception.Message, exception );
            return new ApiReply<bool>( ADMIN_TASK_INTERNAL_SERVER_ERROR );
        }
    }
}