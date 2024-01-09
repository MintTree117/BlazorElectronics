using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Promos;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers.Admin;

[Route( "api/[controller]" )]
[ApiController]
public sealed class AdminPromoController : _AdminController
{
    readonly IPromoService _promoService;
    
    public AdminPromoController( ILogger<_UserController> logger, IUserAccountService userAccountService, ISessionService sessionService, IPromoService promoPromoService )
        : base( logger, userAccountService, sessionService )
    {
        _promoService = promoPromoService;
    }
    
    [HttpGet( "get-view" )]
    public async Task<ActionResult<List<PromoEditDto>?>> GetView()
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );
        
        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );
        
        ServiceReply<List<PromoEditDto>?> reply = await _promoService.GetView();
        return GetReturnFromReply( reply );
    }
    [HttpGet("get-edit")]
    public async Task<ActionResult<PromoEditDto?>> GetEdit( int itemId )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<PromoEditDto?> reply = await _promoService.GetEdit( itemId );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "add" )]
    public async Task<ActionResult<int>> Add( [FromBody] PromoEditDto requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<int> reply = await _promoService.Add( requestDto );
        return GetReturnFromReply( reply );
    }
    [HttpPut( "update" )]
    public async Task<ActionResult<bool>> Update( [FromBody] PromoEditDto requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _promoService.Update( requestDto);
        return GetReturnFromReply( reply );
    }
    [HttpDelete( "remove" )]
    public async Task<ActionResult<bool>> Remove( int itemId )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _promoService.Remove( itemId );
        return GetReturnFromReply( reply );
    }
}