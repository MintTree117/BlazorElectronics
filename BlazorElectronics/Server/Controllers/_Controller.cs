using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

public class _Controller : ControllerBase
{
    protected const string NOT_FOUND_MESSAGE = "Requested data was not found!";
    protected const string BAD_REQUEST_MESSAGE = "Bad Request!";
    protected const string INTERNAL_SERVER_ERROR = "Internal Server Error!";

    protected readonly ILogger<_Controller> Logger;
    
    public _Controller( ILogger<_Controller> logger )
    {
        Logger = logger;
    }

    protected ActionResult GetReturnFromApi<T>( ServiceReply<T> reply )
    {
        if ( reply.Success )
        {
            return Ok( reply.Data );
        }

        return reply.ErrorType switch
        {
            ServiceErrorType.ValidationError => BadRequest( reply.Message ),
            ServiceErrorType.NotFound => NotFound( reply.Message ),
            ServiceErrorType.Unauthorized => Unauthorized( reply.Message ),
            ServiceErrorType.Conflict => Conflict( reply.Message ),
            ServiceErrorType.ServerError => StatusCode( 500, reply.Message ),
            _ => Ok( reply.Data )
        };
    }
}