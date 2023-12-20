using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Cart;
using BlazorElectronics.Shared.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Api.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public sealed class CartController : UserController
{
    readonly ICartService _cartService;

    public CartController( ILogger<UserController> logger, IUserAccountService userAccountService, ISessionService sessionService, ICartService cartService ) : base( logger, userAccountService, sessionService )
    {
        _cartService = cartService;
    }
    
    [HttpPost( "update" )]
    public async Task<ActionResult<List<CartProductDto>?>> UpdateCart( [FromBody] UserDataRequestDto<List<CartItemDto>> requestDto )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId( requestDto );
        
        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<List<CartProductDto>?> reply = await _cartService.UpdateCart( userReply.Data, requestDto.Payload );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "add-update" )]
    public async Task<ActionResult<bool>> AddToCart( [FromBody] UserDataRequestDto<CartItemDto> requestDto )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId( requestDto );

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<bool> reply = await _cartService.AddToCart( userReply.Data, requestDto.Payload );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "remove" )]
    public async Task<ActionResult<bool>> RemoveItemFromCart( [FromBody] UserDataRequestDto<IntDto> requestDto )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId( requestDto );

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<bool> reply = await _cartService.RemoveFromCart( userReply.Data, requestDto.Payload.Value );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "clear" )]
    public async Task<ActionResult<bool>> ClearCart( [FromBody] UserRequestDto requestDto )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId( requestDto );

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<bool> reply = await _cartService.ClearCart( userReply.Data );
        return GetReturnFromReply( reply );
    }
}