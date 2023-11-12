using BlazorElectronics.Server.Services.Cart;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Inbound.Cart;
using BlazorElectronics.Shared.Inbound.Users;
using BlazorElectronics.Shared.Mutual;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class CartController : UserController
{
    readonly ICartService _cartService;

    public CartController( IUserAccountService userAccountService, ISessionService sessionService, ICartService cartService ) : base( userAccountService, sessionService )
    {
        _cartService = cartService;
    }

    [HttpPost( "post" )]
    public async Task<ActionResult<ApiReply<Cart_DTO>>> UpdateCartItems( [FromBody] CartItemsInsertRequest request )
    {
        ApiReply<int> validateReply = await ValidateUserSession( request.ApiRequest );

        if ( !validateReply.Success )
            return BadRequest( new ApiReply<Cart_DTO>( validateReply.Message ) );
        
        ApiReply<Cart_DTO?> cartReply = await _cartService.PostCartItems( validateReply.Data, request.Items );

        return cartReply.Success
            ? Ok( cartReply )
            : BadRequest( new ApiReply<Cart_DTO?>( cartReply.Message ) );
    }
    [HttpPost( "insert" )]
    public async Task<ActionResult<ApiReply<bool>>> AddToCart( [FromBody] CartItemRequest request )
    {
        ApiReply<int> validateReply = await ValidateUserSession( request.ApiRequest );

        if ( !validateReply.Success )
            return BadRequest( new ApiReply<Cart_DTO>( validateReply.Message ) );
        
        ApiReply<bool> cartReply = await _cartService.AddToCart( validateReply.Data, request.CartItemIds! );

        return cartReply.Success
            ? Ok( cartReply )
            : BadRequest( new ApiReply<bool>( cartReply.Message ) );
    }
    [HttpPost( "update-quantity" )]
    public async Task<ActionResult<ApiReply<bool>>> UpdateQuantity( [FromBody] CartItemRequest request )
    {
        ApiReply<int> validateReply = await ValidateUserSession( request.ApiRequest );

        if ( !validateReply.Success )
            return BadRequest( new ApiReply<Cart_DTO>( validateReply.Message ) );

        ApiReply<bool> cartResponse = await _cartService.UpdateQuantity( validateReply.Data, request.CartItemIds! );

        return cartResponse.Success
            ? Ok( cartResponse )
            : BadRequest( cartResponse );
    }
    [HttpPost( "remove" )]
    public async Task<ActionResult<ApiReply<bool>>> RemoveItemFromCart( [FromBody] CartItemRequest request )
    {
        ApiReply<int> validateReply = await ValidateUserSession( request.ApiRequest );

        if ( !validateReply.Success )
            return BadRequest( new ApiReply<Cart_DTO>( validateReply.Message ) );

        ApiReply<bool> cartResponse = await _cartService.RemoveFromCart( validateReply.Data, request.CartItemIds! );

        return cartResponse.Success
            ? Ok( cartResponse )
            : BadRequest( cartResponse );
    }
    [HttpGet( "count" )]
    public async Task<ActionResult<ApiReply<int>>> GetCartItemsCount( [FromBody] SessionApiRequest request )
    {
        ApiReply<int> validateReply = await ValidateUserSession( request );

        if ( !validateReply.Success )
            return BadRequest( new ApiReply<Cart_DTO>( validateReply.Message ) );

        ApiReply<int> cartResponse = await _cartService.CountCartItems( validateReply.Data  );

        return cartResponse.Success
            ? Ok( cartResponse )
            : BadRequest( cartResponse );
    }
    [HttpGet( "products")]
    public async Task<ActionResult<ApiReply<Cart_DTO>>> GetCartProducts( [FromBody] SessionApiRequest request )
    {
        ApiReply<int> validateReply = await ValidateUserSession( request );

        if ( !validateReply.Success )
            return BadRequest( new ApiReply<Cart_DTO>( validateReply.Message ) );

        ApiReply<Cart_DTO?> cartResponse = await _cartService.GetCartProducts( validateReply.Data );

        return cartResponse.Success
            ? Ok( cartResponse )
            : BadRequest( cartResponse );
    }
}