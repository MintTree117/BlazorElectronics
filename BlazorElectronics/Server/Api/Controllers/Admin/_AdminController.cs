using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers.Admin;

[Route( "api/[controller]" )]
[ApiController]
public class _AdminController : UserController
{
    public _AdminController( ILogger<UserController> logger, IUserAccountService userAccountService, ISessionService sessionService )
        : base( logger, userAccountService, sessionService ) { }

    [HttpPost( "authorize" )]
    public async Task<ActionResult<bool>> AuthorizeAdmin( [FromBody] UserRequestDto requestDto )
    {
        ServiceReply<bool> reply = await ValidateAndAuthorizeAdmin( requestDto );
        return GetReturnFromReply( reply );
    }

    protected async Task<ServiceReply<bool>> ValidateAndAuthorizeAdmin( UserRequestDto? request )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId( request );

        if ( !userReply.Success )
            return new ServiceReply<bool>( userReply.ErrorType, userReply.Message );

        return await UserAccountService.VerifyAdmin( userReply.Data );
    }
    protected async Task<ServiceReply<int>> ValidateAndAuthorizeAdminId( UserRequestDto? request )
    { 
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId( request );

        if ( !userReply.Success )
            return userReply;
        
        return await UserAccountService.VerifyAdminId( userReply.Data );
    }
}