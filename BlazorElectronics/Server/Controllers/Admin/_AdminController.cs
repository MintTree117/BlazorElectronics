using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers.Admin;

[Route( "api/[controller]" )]
[ApiController]
public class _AdminController : UserController
{
    public _AdminController( ILogger<UserController> logger, IUserAccountService userAccountService, ISessionService sessionService )
        : base( logger, userAccountService, sessionService ) { }

    [HttpPost( "authorize-admin" )]
    public async Task<ActionResult<bool>> AuthorizeAdmin( [FromBody] UserRequest? request )
    {
        ServiceReply<int> reply = await ValidateAndAuthorizeAdmin( request );
        return GetReturnFromApi( reply );
    }
    
    protected async Task<ServiceReply<int>> ValidateAndAuthorizeAdmin( UserRequest? request )
    { 
        ServiceReply<int> userReply = await ValidateAndAuthorizeUser( request );

        if ( !userReply.Success )
            return userReply;
        
        return await UserAccountService.VerifyAdminId( userReply.Data );
    }
}