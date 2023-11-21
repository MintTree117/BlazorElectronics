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
    public async Task<ActionResult<ApiReply<bool>>> AuthorizeAdmin( [FromBody] UserApiRequest? apiRequest )
    {
        ApiReply<ValidatedUserApiRequest<object?>> reply = await ValidateAdminRequest<object>( apiRequest );

        return reply.Success
            ? Ok( new ApiReply<bool>( true ) )
            : BadRequest( reply.Message );
    }
    
    protected async Task<ApiReply<ValidatedUserApiRequest<T?>>> ValidateAdminRequest<T>( UserApiRequest? request ) where T : class
    {
        ApiReply<ValidatedUserApiRequest<T?>> userRequestReply = await TryValidateUserRequest<T>( request );

        if ( !userRequestReply.Success || userRequestReply.Data is null )
            return new ApiReply<ValidatedUserApiRequest<T?>>( userRequestReply.Message );

        ApiReply<int> adminReply = await UserAccountService.VerifyAdminId( userRequestReply.Data.UserId );

        return adminReply.Success
            ? userRequestReply
            : new ApiReply<ValidatedUserApiRequest<T?>>( adminReply.Message );
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