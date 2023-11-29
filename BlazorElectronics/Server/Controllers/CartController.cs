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
    public async Task<ActionResult<CartResponse?>> UpdateCart( [FromBody] UserDataRequest<CartRequest>? request )
    {
        ApiReply<int> userReply = await ValidateAndAuthorizeUser( request );
        
        if ( !userReply.Success )
            return GetReturnFromApi( userReply );
    }
    [HttpPost( "insert" )]
    public async Task<ActionResult<bool>> AddToCart( [FromBody] UserDataRequest<CartItemDto>? request )
    {
        ApiReply<int> userReply = await ValidateAndAuthorizeUser( request );

        if ( !userReply.Success )
            return GetReturnFromApi( userReply );
    }
    [HttpPost( "update-quantity" )]
    public async Task<ActionResult<bool>> UpdateQuantity( [FromBody] UserDataRequest<CartItemDto>? request )
    {
        ApiReply<int> userReply = await ValidateAndAuthorizeUser( request );

        if ( !userReply.Success )
            return GetReturnFromApi( userReply );
    }
    [HttpPost( "remove" )]
    public async Task<ActionResult<bool>> RemoveItemFromCart( [FromBody] UserDataRequest<IntDto>? request )
    {
        ApiReply<int> userReply = await ValidateAndAuthorizeUser( request );

        if ( !userReply.Success )
            return GetReturnFromApi( userReply );
    }
    [HttpGet( "count" )]
    public async Task<ActionResult<int>> GetCartItemsCount( [FromBody] UserRequest? request )
    {
        ApiReply<int> userReply = await ValidateAndAuthorizeUser( request );

        if ( !userReply.Success )
            return GetReturnFromApi( userReply );
    }
    [HttpGet( "products")]
    public async Task<ActionResult<CartResponse?>> GetCartProducts( [FromBody] UserRequest? request )
    {
        ApiReply<int> userReply = await ValidateAndAuthorizeUser( request );

        if ( !userReply.Success )
            return GetReturnFromApi( userReply );
    }
}