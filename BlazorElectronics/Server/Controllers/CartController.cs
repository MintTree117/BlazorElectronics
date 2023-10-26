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
    public async Task<ActionResult<ServiceResponse<Cart_DTO>>> UpdateCartItems( CartItemsInsertRequest request )
    {
        if ( !ValidateApiRequest( request.ApiRequest, out string? ipAddress, out string message ) )
            return BadRequest( new ServiceResponse<Cart_DTO>( null, false, message ) );

        ServiceResponse<ValidatedIdAndSession> validateSessionResponse = await ValidateUserSession( request.ApiRequest!, ipAddress! );

        if ( !validateSessionResponse.Success )
            return BadRequest( new ServiceResponse<Cart_DTO>( null, false, validateSessionResponse.Message! ) );
        
        ServiceResponse<Cart_DTO?> cartResponse = await _cartService.PostCartItems( validateSessionResponse.Data!.UserId, request.Items );

        return cartResponse.Success
            ? Ok( cartResponse )
            : BadRequest( cartResponse );
    }
    [HttpPost( "insert" )]
    public async Task<ActionResult<ServiceResponse<bool>>> AddToCart( CartItemRequest request )
    {
        if ( !ValidateApiRequest( request.ApiRequest, out string? ipAddress, out string message ) )
            return BadRequest( new ServiceResponse<Cart_DTO>( null, false, message ) );

        ServiceResponse<ValidatedIdAndSession> validateSessionResponse = await ValidateUserSession( request.ApiRequest!, ipAddress! );

        if ( !validateSessionResponse.Success )
            return BadRequest( new ServiceResponse<Cart_DTO>( null, false, validateSessionResponse.Message! ) );
        
        ServiceResponse<bool> cartResponse = await _cartService.AddToCart( validateSessionResponse.Data!.UserId, request.CartItemIds! );

        return cartResponse.Success
            ? Ok( cartResponse )
            : BadRequest( cartResponse );
    }
    [HttpPost( "update-quantity" )]
    public async Task<ActionResult<ServiceResponse<bool>>> UpdateQuantity( CartItemRequest request )
    {
        if ( !ValidateApiRequest( request.ApiRequest, out string? ipAddress, out string message ) )
            return BadRequest( new ServiceResponse<Cart_DTO>( null, false, message ) );

        ServiceResponse<ValidatedIdAndSession> validateSessionResponse = await ValidateUserSession( request.ApiRequest!, ipAddress! );

        if ( !validateSessionResponse.Success )
            return BadRequest( new ServiceResponse<Cart_DTO>( null, false, validateSessionResponse.Message! ) );

        ServiceResponse<bool> cartResponse = await _cartService.UpdateQuantity( validateSessionResponse.Data!.UserId, request.CartItemIds! );

        return cartResponse.Success
            ? Ok( cartResponse )
            : BadRequest( cartResponse );
    }
    [HttpPost( "remove" )]
    public async Task<ActionResult<ServiceResponse<bool>>> RemoveItemFromCart( CartItemRequest request )
    {
        if ( !ValidateApiRequest( request.ApiRequest, out string? ipAddress, out string message ) )
            return BadRequest( new ServiceResponse<Cart_DTO>( null, false, message ) );

        ServiceResponse<ValidatedIdAndSession> validateSessionResponse = await ValidateUserSession( request.ApiRequest!, ipAddress! );

        if ( !validateSessionResponse.Success )
            return BadRequest( new ServiceResponse<Cart_DTO>( null, false, validateSessionResponse.Message! ) );

        ServiceResponse<bool> cartResponse = await _cartService.RemoveFromCart( validateSessionResponse.Data!.UserId, request.CartItemIds! );

        return cartResponse.Success
            ? Ok( cartResponse )
            : BadRequest( cartResponse );
    }
    [HttpGet( "count" )]
    public async Task<ActionResult<ServiceResponse<int>>> GetCartItemsCount( SessionApiRequest request )
    {
        if ( !ValidateApiRequest( request, out string? ipAddress, out string message ) )
            return BadRequest( new ServiceResponse<Cart_DTO>( null, false, message ) );

        ServiceResponse<ValidatedIdAndSession> validateSessionResponse = await ValidateUserSession( request, ipAddress! );

        if ( !validateSessionResponse.Success )
            return BadRequest( new ServiceResponse<Cart_DTO>( null, false, validateSessionResponse.Message! ) );

        ServiceResponse<int> cartResponse = await _cartService.CountCartItems( validateSessionResponse.Data!.UserId  );

        return cartResponse.Success
            ? Ok( cartResponse )
            : BadRequest( cartResponse );
    }
    [HttpGet( "products")]
    public async Task<ActionResult<ServiceResponse<Cart_DTO>>> GetCartProducts( SessionApiRequest request )
    {
        if ( !ValidateApiRequest( request, out string? ipAddress, out string message ) )
            return BadRequest( new ServiceResponse<Cart_DTO>( null, false, message ) );

        ServiceResponse<ValidatedIdAndSession> validateSessionResponse = await ValidateUserSession( request, ipAddress! );

        if ( !validateSessionResponse.Success )
            return BadRequest( new ServiceResponse<Cart_DTO>( null, false, validateSessionResponse.Message! ) );

        ServiceResponse<Cart_DTO?> cartResponse = await _cartService.GetCartProducts( validateSessionResponse.Data!.UserId );

        return cartResponse.Success
            ? Ok( cartResponse )
            : BadRequest( cartResponse );
    }
}