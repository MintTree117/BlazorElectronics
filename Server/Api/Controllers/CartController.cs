using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Shared.Cart;
using BlazorElectronics.Shared.Promos;
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
    
    [HttpPost( "update-cart" )]
    public async Task<ActionResult<CartDto?>> UpdateCart( [FromBody] List<CartItemDto> requestDto )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId();
        
        if ( !userReply.Success )
            return GetReturnFromReply( userReply );
        
        ServiceReply<CartDto?> reply = await _cartService.UpdateCart( userReply.Data, requestDto );
        return GetReturnFromReply( reply );
    }
    [HttpPut( "add-update-item" )]
    public async Task<ActionResult<bool>> AddToCart( [FromBody] CartItemDto requestDto )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId();

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<bool> reply = await _cartService.AddToCart( userReply.Data, requestDto );
        return GetReturnFromReply( reply );
    }
    [HttpDelete( "remove-item" )]
    public async Task<ActionResult<bool>> RemoveItemFromCart( int itemId )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId();

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<bool> reply = await _cartService.RemoveFromCart( userReply.Data, itemId );
        return GetReturnFromReply( reply );
    }
    [HttpDelete( "clear-cart" )]
    public async Task<ActionResult<bool>> ClearCart()
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId();

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<bool> reply = await _cartService.ClearCart( userReply.Data );
        return GetReturnFromReply( reply );
    }
    [HttpPut( "add-promo" )]
    public async Task<ActionResult<PromoCodeDto?>> AddPromo( [FromBody] string promoCode )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId();

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<PromoCodeDto?> reply = await _cartService.AddPromo( userReply.Data, promoCode );
        return GetReturnFromReply( reply );
    }
    [HttpDelete( "remove-promo" )]
    public async Task<ActionResult<bool>> RemovePromo( int promoId )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId();

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<bool> reply = await _cartService.RemovePromo( userReply.Data, promoId );
        return GetReturnFromReply( reply );
    }
}