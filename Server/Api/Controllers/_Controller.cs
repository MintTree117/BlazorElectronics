using BlazorElectronics.Shared.Enums;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers;

public abstract class _Controller : ControllerBase
{
    protected readonly ILogger<_Controller> Logger;

    protected _Controller( ILogger<_Controller> logger )
    {
        Logger = logger;
    }

    protected ActionResult GetReturnFromReply<T>( ServiceReply<T> reply )
    {
        if ( reply.Success )
        {
            return Ok( reply.Payload );
        }

        return reply.ErrorType switch
        {
            ServiceErrorType.ValidationError => BadRequest( reply.Message ),
            ServiceErrorType.NotFound => NotFound( reply.Message ),
            ServiceErrorType.Unauthorized => Unauthorized( reply.Message ),
            ServiceErrorType.Conflict => Conflict( reply.Message ),
            ServiceErrorType.ServerError => StatusCode( 500, reply.Message ),
            _ => Ok( reply.Payload )
        };
    }
}