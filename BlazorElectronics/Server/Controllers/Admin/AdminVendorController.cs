using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Server.Services.Vendors;
using BlazorElectronics.Shared.Admin.Vendors;
using BlazorElectronics.Shared.Users;
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
    
    [HttpPost( "get-vendor-view" )]
    public async Task<ActionResult<VendorsViewDto?>> GetView( [FromBody] UserRequest? request )
    {
        ApiReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );
        
        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ApiReply<VendorsViewDto?> reply = await _vendorService.GetView();
        return GetReturnFromApi( reply );
    }
    [HttpPost("get-vendor-edit")]
    public async Task<ActionResult<VendorEditDto?>> GetEdit( [FromBody] UserDataRequest<IntDto>? request )
    {
        ApiReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ApiReply<VendorEditDto?> reply = await _vendorService.GetEdit( request!.Payload!.Value );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "add-vendor" )]
    public async Task<ActionResult<int>> Add( [FromBody] UserDataRequest<VendorEditDto>? request )
    {
        ApiReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ApiReply<int> reply = await _vendorService.Add( request!.Payload! );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "update-vendor" )]
    public async Task<ActionResult<bool>> Update( [FromBody] UserDataRequest<VendorEditDto>? request )
    {
        ApiReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ApiReply<bool> reply = await _vendorService.Update( request!.Payload! );
        return GetReturnFromApi( reply );
    }
    [HttpPost( "remove-vendor" )]
    public async Task<ActionResult<bool>> Remove( [FromBody] UserDataRequest<IntDto>? request )
    {
        ApiReply<int> adminReply = await ValidateAndAuthorizeAdmin( request );

        if ( !adminReply.Success )
            return GetReturnFromApi( adminReply );

        ApiReply<bool> reply = await _vendorService.Remove( request!.Payload!.Value );
        return GetReturnFromApi( reply );
    }
}