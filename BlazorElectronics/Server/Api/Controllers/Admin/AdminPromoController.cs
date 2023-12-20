using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Promos;
using BlazorElectronics.Shared.Users;
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
    
    [HttpPost( "get-view" )]
    public async Task<ActionResult<List<PromoEditDto>?>> GetCategoriesView( [FromBody] UserRequestDto requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );
        
        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<List<PromoEditDto>?> reply = await _service.GetView();
        return GetReturnFromReply( reply );
    }
    [HttpPost("get-edit")]
    public async Task<ActionResult<PromoEditDto?>> GetCategoryForEdit( [FromBody] UserDataRequestDto<IntDto> requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<PromoEditDto?> reply = await _service.GetEdit( requestDto.Payload.Value );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "add" )]
    public async Task<ActionResult<int>> AddCategory( [FromBody] UserDataRequestDto<PromoEditDto> requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<int> reply = await _service.Add( requestDto.Payload );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "update" )]
    public async Task<ActionResult<bool>> UpdateCategory( [FromBody] UserDataRequestDto<PromoEditDto> requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _service.Update( requestDto.Payload );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "remove" )]
    public async Task<ActionResult<bool>> RemoveCategory( [FromBody] UserDataRequestDto<IntDto> requestDto )
    {
        ServiceReply<int> adminReply = await ValidateAndAuthorizeAdminId( requestDto );

        if ( !adminReply.Success )
            return GetReturnFromReply( adminReply );

        ServiceReply<bool> reply = await _service.Remove( requestDto.Payload.Value );
        return GetReturnFromReply( reply );
    }
}