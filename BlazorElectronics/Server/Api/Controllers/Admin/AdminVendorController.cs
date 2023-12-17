using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Users;
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
    
    [HttpPost( "get-view" )]
    public async Task<ActionResult<List<CrudViewDto>?>> GetView( [FromBody] UserRequestDto requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );
        
        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<List<CrudViewDto>?> reply = await _vendorService.GetView();
        return GetReturnFromReply( reply );
    }
    [HttpPost("get-edit")]
    public async Task<ActionResult<VendorEditDtoDto?>> GetEdit( [FromBody] UserDataRequestDto<IntDto> requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<VendorEditDtoDto?> reply = await _vendorService.GetEdit( requestDto.Payload.Value );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "add" )]
    public async Task<ActionResult<int>> Add( [FromBody] UserDataRequestDto<VendorEditDtoDto> requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<int> reply = await _vendorService.Add( requestDto.Payload );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "update" )]
    public async Task<ActionResult<bool>> Update( [FromBody] UserDataRequestDto<VendorEditDtoDto> requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _vendorService.Update( requestDto.Payload );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "remove" )]
    public async Task<ActionResult<bool>> Remove( [FromBody] UserDataRequestDto<IntDto> requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _vendorService.Remove( requestDto.Payload.Value );
        return GetReturnFromReply( reply );
    }
}