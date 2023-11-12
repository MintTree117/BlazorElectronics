using BlazorElectronics.Shared.Inbound.Cart;
using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Server.Services.Cart;

public interface ICartService
{
    Task<ApiReply<Cart_DTO?>> PostCartItems( int userId, List<CartItemId_DTO> cartItemsDtos );
    Task<ApiReply<bool>> AddToCart( int userId, CartItemId_DTO cartItem );
    Task<ApiReply<bool>> UpdateQuantity( int userId, CartItemId_DTO cartItem );
    Task<ApiReply<bool>> RemoveFromCart( int userId, CartItemId_DTO cartItem );
    Task<ApiReply<Cart_DTO?>> GetCartProducts( int userId );
    Task<ApiReply<int>> CountCartItems( int userId );
}