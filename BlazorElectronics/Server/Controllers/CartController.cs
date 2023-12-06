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
    
    [HttpGet( "products" )]
    public async Task<ActionResult<CartResponse?>> GetCartProducts( [FromBody] UserRequest request )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId( request );

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<CartResponse?> reply = await _cartService.GetCart( userReply.Data );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "post" )]
    public async Task<ActionResult<CartResponse?>> UpdateCart( [FromBody] UserDataRequest<CartRequest> request )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId( request );
        
        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<CartResponse?> reply = await _cartService.UpdateCart( userReply.Data, request!.Payload! );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "insert" )]
    public async Task<ActionResult<CartResponse?>> AddToCart( [FromBody] UserDataRequest<CartItem> request )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId( request );

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<CartResponse?> reply = await _cartService.AddToCart( userReply.Data, request!.Payload! );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "update-quantity" )]
    public async Task<ActionResult<CartResponse?>> UpdateQuantity( [FromBody] UserDataRequest<CartItem> request )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId( request );

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<CartResponse?> reply = await _cartService.UpdateQuantity( userReply.Data, request.Payload );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "remove" )]
    public async Task<ActionResult<CartResponse?>> RemoveItemFromCart( [FromBody] UserDataRequest<IntDto> request )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId( request );

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<CartResponse?> reply = await _cartService.RemoveFromCart( userReply.Data, request!.Payload!.Value );
        return GetReturnFromReply( reply );
    }
    [HttpPost( "clear" )]
    public async Task<ActionResult<bool>> ClearCart( [FromBody] UserRequest request )
    {
        ServiceReply<int> userReply = await ValidateAndAuthorizeUserId( request );

        if ( !userReply.Success )
            return GetReturnFromReply( userReply );

        ServiceReply<bool> reply = await _cartService.ClearCart( userReply.Data );
        return GetReturnFromReply( reply );
    }
}