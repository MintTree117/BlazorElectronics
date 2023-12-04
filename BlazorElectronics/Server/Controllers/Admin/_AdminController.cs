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

    [HttpPost( "authorize" )]
    public async Task<ActionResult<bool>> AuthorizeAdmin( [FromBody] UserRequest? request )
    {
        ServiceReply<bool> reply = await ValidateAndAuthorizeAdmin( request );
        return GetReturnFromApi( reply );
    }

    protected async Task<ServiceReply<bool>> ValidateAndAuthorizeAdmin( UserRequest? request )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId( request );

        if ( !userReply.Success )
            return new ServiceReply<bool>( userReply.ErrorType, userReply.Message );

        return await UserAccountService.VerifyAdmin( userReply.Data );
    }
    protected async Task<ServiceReply<int>> ValidateAndAuthorizeAdminId( UserRequest? request )
    { 
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId( request );

        if ( !userReply.Success )
            return userReply;
        
        return await UserAccountService.VerifyAdminId( userReply.Data );
    }
}