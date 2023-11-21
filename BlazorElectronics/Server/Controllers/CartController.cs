using BlazorElectronics.Server.Dtos.Users;
using BlazorElectronics.Server.Services.Cart;
using BlazorElectronics.Server.Services.Sessions;
using BlazorElectronics.Server.Services.Users;
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
    public async Task<ActionResult<ApiReply<CartResponse>>> UpdateCartItems( [FromBody] UserApiRequest? apiRequest )
    {
        ApiReply<ValidatedUserApiRequest<object?>> validateReply = await TryValidateUserRequest<object>( apiRequest );

        if ( !validateReply.Success || validateReply.Data is null )
            return BadRequest( validateReply.Message );
        
        ApiReply<CartResponse?> cartReply = await _cartService.PostCartItems( validateReply.Data.UserId, null );

        return cartReply.Success
            ? Ok( cartReply )
            : BadRequest( new ApiReply<CartResponse?>( cartReply.Message ) );
    }
    [HttpPost( "insert" )]
    public async Task<ActionResult<ApiReply<bool>>> AddToCart( [FromBody] UserApiRequest? apiRequest )
    {
        ApiReply<ValidatedUserApiRequest<object?>> validateReply = await TryValidateUserRequest<object>( apiRequest );

        if ( !validateReply.Success || validateReply.Data is null )
            return BadRequest( validateReply.Message );
        
        ApiReply<bool> cartReply = await _cartService.AddToCart( validateReply.Data.UserId, null );

        return cartReply.Success
            ? Ok( cartReply )
            : BadRequest( new ApiReply<bool>( cartReply.Message ) );
    }
    [HttpPost( "update-quantity" )]
    public async Task<ActionResult<ApiReply<bool>>> UpdateQuantity( [FromBody] UserApiRequest? apiRequest )
    {
        ApiReply<ValidatedUserApiRequest<object?>> validateReply = await TryValidateUserRequest<object>( apiRequest );

        if ( !validateReply.Success || validateReply.Data is null )
            return BadRequest( validateReply.Message );

        ApiReply<bool> cartResponse = await _cartService.UpdateQuantity( validateReply.Data.UserId, null );

        return cartResponse.Success
            ? Ok( cartResponse )
            : BadRequest( cartResponse );
    }
    [HttpPost( "remove" )]
    public async Task<ActionResult<ApiReply<bool>>> RemoveItemFromCart( [FromBody] UserApiRequest? apiRequest )
    {
        ApiReply<ValidatedUserApiRequest<object?>> validateReply = await TryValidateUserRequest<object>( apiRequest );

        if ( !validateReply.Success || validateReply.Data is null )
            return BadRequest( validateReply.Message );

        ApiReply<bool> cartResponse = await _cartService.RemoveFromCart( validateReply.Data.UserId, null );

        return cartResponse.Success
            ? Ok( cartResponse )
            : BadRequest( cartResponse );
    }
    [HttpGet( "count" )]
    public async Task<ActionResult<ApiReply<int>>> GetCartItemsCount( [FromBody] UserApiRequest? apiRequest )
    {
        ApiReply<ValidatedUserApiRequest<object?>> validateReply = await TryValidateUserRequest<object>( apiRequest );

        if ( !validateReply.Success || validateReply.Data is null )
            return BadRequest( validateReply.Message );

        ApiReply<int> cartResponse = await _cartService.CountCartItems( validateReply.Data.UserId );

        return cartResponse.Success
            ? Ok( cartResponse )
            : BadRequest( cartResponse );
    }
    [HttpGet( "products")]
    public async Task<ActionResult<ApiReply<CartResponse>>> GetCartProducts( [FromBody] UserApiRequest? apiRequest )
    {
        ApiReply<ValidatedUserApiRequest<object?>> validateReply = await TryValidateUserRequest<object>( apiRequest );

        if ( !validateReply.Success || validateReply.Data is null )
            return BadRequest( validateReply.Message );

        ApiReply<CartResponse?> cartResponse = await _cartService.GetCartProducts( validateReply.Data.UserId );

        return cartResponse.Success
            ? Ok( cartResponse )
            : BadRequest( cartResponse );
    }
}