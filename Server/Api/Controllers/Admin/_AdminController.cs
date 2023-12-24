using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers.Admin;

[Route( "api/[controller]" )]
[ApiController]
public class _AdminController : UserController
{
    public _AdminController( ILogger<UserController> logger, IUserAccountService userAccountService, ISessionService sessionService )
        : base( logger, userAccountService, sessionService ) { }

    [HttpGet( "authorize" )]
    public async Task<ActionResult<bool>> AuthorizeAdmin()
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );
        
        Logger.LogError( $"Admin authorized: {adminReply.Success}" );
        
        return adminReply.Success
            ? true
            : GetReturnFromReply( adminReply );
    }
}