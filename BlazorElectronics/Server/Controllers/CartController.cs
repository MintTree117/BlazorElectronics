using BlazorElectronics.Server.Services.Cart;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
using BlazorElectronics.Shared.Cart;
using BlazorElectronics.Shared.Users;
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
        HttpAuthorization authorized = await ValidateAndAuthorizeUser( request );

        return authorized.HttpError ?? Ok( await _cartService.PostCartItems( authorized.UserId, null ) );
    }
    [HttpPost( "insert" )]
    public async Task<ActionResult<ApiReply<bool>>> AddToCart( [FromBody] UserDataRequest<CartItemIdsDto>? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeUser( request );

        return authorized.HttpError ?? Ok( await _cartService.AddToCart( authorized.UserId, request!.Payload! ) );
    }
    [HttpPost( "update-quantity" )]
    public async Task<ActionResult<ApiReply<bool>>> UpdateQuantity( [FromBody] UserDataRequest<CartItemIdsDto>? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeUser( request );

        return authorized.HttpError ?? Ok( await _cartService.UpdateQuantity( authorized.UserId, request!.Payload! ) );
    }
    [HttpPost( "remove" )]
    public async Task<ActionResult<ApiReply<bool>>> RemoveItemFromCart( [FromBody] UserDataRequest<CartItemIdsDto>? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeUser( request );

        return authorized.HttpError ?? Ok( await _cartService.RemoveFromCart( authorized.UserId, request!.Payload! ) );
    }
    [HttpGet( "count" )]
    public async Task<ActionResult<ApiReply<int>>> GetCartItemsCount( [FromBody] UserRequest? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeUser( request );

        return authorized.HttpError ?? Ok( await _cartService.CountCartItems( authorized.UserId ) );
    }
    [HttpGet( "products")]
    public async Task<ActionResult<ApiReply<CartResponse?>>> GetCartProducts( [FromBody] UserRequest? request )
    {
        HttpAuthorization authorized = await ValidateAndAuthorizeUser( request );

        return authorized.HttpError ?? Ok( await _cartService.GetCartProducts( authorized.UserId ) );
    }
}