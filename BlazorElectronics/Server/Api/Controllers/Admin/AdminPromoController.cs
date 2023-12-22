using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Promos;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers.Admin;

[Route( "api/[controller]" )]
[ApiController]
public sealed class AdminPromoController : _AdminController
{
    readonly IPromoService _service;
    
    public AdminPromoController( ILogger<UserController> logger, IUserAccountService userAccountService, ISessionService sessionService, IPromoService promoService )
        : base( logger, userAccountService, sessionService )
    {
        _service = promoService;
    }
    
    [HttpGet( "get-view" )]
    public async Task<ActionResult<List<PromoEditDto>?>> GetCategoriesView()
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<List<PromoEditDto>?> reply = await _service.GetView();
        return GetReturnFromReply( reply );
    }
    [HttpGet("get-edit")]
    public async Task<ActionResult<PromoEditDto?>> GetCategoryForEdit( int itemId )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<PromoEditDto?> reply = await _service.GetEdit( itemId );
        return GetReturnFromReply( reply );
    }
    [HttpPut( "add" )]
    public async Task<ActionResult<int>> AddCategory( [FromBody] PromoEditDto requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<int> reply = await _service.Add( requestDto );
        return GetReturnFromReply( reply );
    }
    [HttpPut( "update" )]
    public async Task<ActionResult<bool>> UpdateCategory( [FromBody] PromoEditDto requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _service.Update( requestDto);
        return GetReturnFromReply( reply );
    }
    [HttpDelete( "remove" )]
    public async Task<ActionResult<bool>> RemoveCategory( int itemId )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeUserId( true );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _service.Remove( itemId );
        return GetReturnFromReply( reply );
    }
}