using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Server.Services.Vendors;
using BlazorElectronics.Shared.Users;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers.Admin;

[Route( "api/[controller]" )]
[ApiController]
public sealed class AdminVendorController : _AdminController
{
    readonly IVendorService _vendorService;

    public AdminVendorController( ILogger<UserController> logger, IUserAccountService userAccountService, ISessionService sessionService, IVendorService vendorService )
        : base( logger, userAccountService, sessionService )
    {
        _vendorService = vendorService;
    }
    
    [HttpPost( "get-view" )]
    public async Task<ActionResult<List<CrudView>?>> GetView( [FromBody] UserRequest request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );
        
        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ServiceReply<List<CrudView>?> reply = await _vendorService.GetView();
        return GetReturnFromApi( reply );
    }
    [HttpPost("get-edit")]
    public async Task<ActionResult<VendorEdit?>> GetEdit( [FromBody] UserDataRequest<IntDto> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ServiceReply<VendorEdit?> reply = await _vendorService.GetEdit( request.Payload.Value );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "add" )]
    public async Task<ActionResult<int>> Add( [FromBody] UserDataRequest<VendorEdit> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ServiceReply<int> reply = await _vendorService.Add( request.Payload );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "update" )]
    public async Task<ActionResult<bool>> Update( [FromBody] UserDataRequest<VendorEdit> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ServiceReply<bool> reply = await _vendorService.Update( request.Payload );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "remove" )]
    public async Task<ActionResult<bool>> Remove( [FromBody] UserDataRequest<IntDto> request )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ServiceReply<bool> reply = await _vendorService.Remove( request.Payload.Value );
        return GetReturnFromApi( reply );
    }
}