using BlazorElectronics.Server.Dtos.Users;
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

    public CartController( ILogger<UserController> logger, IUserAccountService userAccountService, ISessionService sessionService, ICartService cartService ) : base( logger, userAccountService, sessionService )
    {
        _cartService = cartService;
    }

    [HttpPost( "post" )]
    public async Task<ActionResult<ApiReply<CartResponse?>>> UpdateCartItems( [FromBody] UserDataRequest<CartResponse>? request )
    {
        ApiReply<int> validateReply = await AuthorizeSessionRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        return Ok( await _cartService.PostCartItems( validateReply.Data, null ) );
    }
    [HttpPost( "insert" )]
    public async Task<ActionResult<ApiReply<bool>>> AddToCart( [FromBody] UserDataRequest<CartItemId_DTO>? request )
    {
        ApiReply<int> validateReply = await AuthorizeSessionRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        return Ok( await _cartService.AddToCart( validateReply.Data, request!.Payload! ) );
    }
    [HttpPost( "update-quantity" )]
    public async Task<ActionResult<ApiReply<bool>>> UpdateQuantity( [FromBody] UserDataRequest<CartItemId_DTO>? request )
    {
        ApiReply<int> validateReply = await AuthorizeSessionRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        return Ok( await _cartService.UpdateQuantity( validateReply.Data, request!.Payload! ) );
    }
    [HttpPost( "remove" )]
    public async Task<ActionResult<ApiReply<bool>>> RemoveItemFromCart( [FromBody] UserDataRequest<CartItemId_DTO>? request )
    {
        ApiReply<int> validateReply = await AuthorizeSessionRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        return Ok( await _cartService.RemoveFromCart( validateReply.Data, request!.Payload! ) );
    }
    [HttpGet( "count" )]
    public async Task<ActionResult<ApiReply<int>>> GetCartItemsCount( [FromBody] UserRequest? request )
    {
        ApiReply<int> validateReply = await AuthorizeSessionRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        return Ok( await _cartService.CountCartItems( validateReply.Data ) );
    }
    [HttpGet( "products")]
    public async Task<ActionResult<ApiReply<CartResponse?>>> GetCartProducts( [FromBody] UserRequest? request )
    {
        ApiReply<int> validateReply = await AuthorizeSessionRequest( request );

        if ( !validateReply.Success )
            return BadRequest( validateReply.Message );

        return Ok( await _cartService.GetCartProducts( validateReply.Data ) );
    }
}