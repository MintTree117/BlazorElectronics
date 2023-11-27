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
    public async Task<ActionResult<ApiReply<bool>>> AuthorizeAdmin( [FromBody] UserRequest? request )
    {
        HttpAuthorization result = await ValidateAndAuthorizeAdmin( request );
        return result.HttpError ?? Ok( new ApiReply<bool>( true ) );
    }
    
    protected async Task<HttpAuthorization> ValidateAndAuthorizeAdmin( UserRequest? request )
    {
        HttpAuthorization result = await ValidateAndAuthorizeUser( request );

        if ( result.HttpError is not null )
            return result;

        return ( await UserAccountService.VerifyAdminId( result.UserId ) ).Success
            ? new HttpAuthorization( result.UserId )
            : new HttpAuthorization( -1, Unauthorized() );
    }
}