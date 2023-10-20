using BlazorElectronics.Server.Services.Cart;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DtosOutbound.Products;
using BlazorElectronics.Shared.Inbound.Cart;
using BlazorElectronics.Shared.Mutual;
using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronics.Server.Controllers;

[Route( "api/[controller]" )]
[ApiController]
public class CartController : ControllerBase
{
    readonly ICartService _cartService;

    public CartController( ICartService cartService )
    {
        _cartService = cartService;
    }

    [HttpPost( "items" )]
    public async Task<ActionResult<ServiceResponse<Products_DTO>>> GetCartProducts( CartItemIds clientItems )
    {
        ServiceResponse<Cart_DTO?> response = await _cartService.GetOrUpdateCartItems( clientItems );
        return Ok( response );
    }
}