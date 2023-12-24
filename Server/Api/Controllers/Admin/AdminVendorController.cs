using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Vendors;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers.Admin;

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
    
    [HttpGet( "get-view" )]
    public async Task<ActionResult<List<CrudViewDto>?>> GetView()
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<List<CrudViewDto>?> reply = await _vendorService.GetView();
        return GetReturnFromReply( reply );
    }
    [HttpGet("get-edit")]
    public async Task<ActionResult<VendorEditDtoDto?>> GetEdit( int itemId )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<VendorEditDtoDto?> reply = await _vendorService.GetEdit( itemId );
        return GetReturnFromReply( reply );
    }
    [HttpPut( "add" )]
    public async Task<ActionResult<int>> Add( [FromBody] VendorEditDtoDto requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<int> reply = await _vendorService.Add( requestDto );
        return GetReturnFromReply( reply );
    }
    [HttpPut( "update" )]
    public async Task<ActionResult<bool>> Update( [FromBody] VendorEditDtoDto requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _vendorService.Update( requestDto );
        return GetReturnFromReply( reply );
    }
    [HttpDelete( "remove" )]
    public async Task<ActionResult<bool>> Remove( [FromBody] int itemId )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _vendorService.Remove( itemId );
        return GetReturnFromReply( reply );
    }
}