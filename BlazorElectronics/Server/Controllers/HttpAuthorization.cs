using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

public sealed class HttpAuthorization
{
    public HttpAuthorization( int id, ActionResult? result = null )
    {
        UserId = id;
        HttpError = result;
    }

    public int UserId { get; init; }
    public ActionResult? HttpError { get; init; }
}