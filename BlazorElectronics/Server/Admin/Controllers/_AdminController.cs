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
    
    const string INVALID_ADMIN_TASK_MESSAGE = "Invalid Task Call!";
    const string ADMIN_TASK_FAIL_MESSAGE = "Task Failed To Execute!";
    const string ADMIN_TASK_INTERNAL_SERVER_ERROR = "An internal server error occured!";


    public _AdminController( ILogger<UserController> logger, IUserAccountService userAccountService, ISessionService sessionService )
        : base( logger, userAccountService, sessionService ) { }

    [HttpPost( "authorize" )]
    public async Task<ActionResult<ApiReply<bool>>> AuthorizeAdmin( [FromBody] UserRequest? request )
    {
        ApiReply<int> reply = await ValidateAdminRequest( request );
        
        return reply.Success
            ? Ok( new ApiReply<bool>( true ) )
            : BadRequest( reply.Message );
    }

    protected async Task<ApiReply<int>> ValidateAdminRequest( UserRequest? request )
    {
        ApiReply<int> userReply = await AuthorizeSessionRequest( request );

        if ( !userReply.Success )
            return userReply;

        return await UserAccountService.VerifyAdminId( userReply.Data );
    }
    protected async Task<ApiReply<int>> ValidateAdminRequest<T>( UserDataRequest<T>? request ) where T : class
    {
        ApiReply<int> userReply = await AuthorizeSessionRequest( request );

        if ( !userReply.Success )
            return userReply;

        return await UserAccountService.VerifyAdminId( userReply.Data );
    }
    protected async Task<ApiReply<T?>> TryExecuteAdminRepoQuery<T>( Delegate action, params object?[] args )
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
            var ex = new ServiceException( e.Message, e );
            
            Logger.LogError( ex.Message );
            return new ApiReply<T?>( ADMIN_TASK_INTERNAL_SERVER_ERROR );
        }
        catch ( IOException e )
        {
            var ex = new ServiceException( e.Message, e );
            Logger.LogError( ex.Message );
            return new ApiReply<T?>( ADMIN_TASK_INTERNAL_SERVER_ERROR );
        }
        catch ( SqlException e )
        {
            var ex = new ServiceException( e.Message, e );
            Logger.LogError( ex.Message );
            return new ApiReply<T?>( ADMIN_TASK_INTERNAL_SERVER_ERROR );
        }
        catch ( Exception e )
        {
            var ex = new ServiceException( e.Message, e );
            Logger.LogError( ex.Message );
            return new ApiReply<T?>( ADMIN_TASK_INTERNAL_SERVER_ERROR );
        }
    }
    protected async Task<ApiReply<bool>> TryExecuteAdminRepoTransaction( Delegate action, params object?[] args )
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