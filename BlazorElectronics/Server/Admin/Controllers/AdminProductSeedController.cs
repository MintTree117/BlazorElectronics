using BlazorElectronics.Server.Controllers;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Admin.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class AdminProductSeedController : _AdminController
{
    public AdminProductSeedController( ILogger<UserController> logger, IUserAccountService userAccountService, ISessionService sessionService )
        : base( logger, userAccountService, sessionService ) { }

    [HttpPost( "seed-products" )]
    public async Task<ActionResult<ApiReply<bool>>> SeedProducts( [FromBody] UserDataRequest<IdDto> request )
    {
        return null;
    }
}