using BlazorElectronics.Server.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers.Admin;

[Route( "api/[controller]" )]
[ApiController]
public class _AdminController : _UserController
{
    public _AdminController( ILogger<_UserController> logger, IUserAccountService userAccountService, ISessionService sessionService )
        : base( logger, userAccountService, sessionService ) { }

    [HttpGet( "authorize-admin" )]
    public async Task<ActionResult<bool>> AuthorizeAdmin()
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );
        
        return adminReply.Success
            ? Ok( true )
            : GetReturnFromReply( adminReply );
    }
}