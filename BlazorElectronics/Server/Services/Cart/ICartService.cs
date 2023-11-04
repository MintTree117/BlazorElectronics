using BlazorElectronics.Shared.Inbound.Cart;
using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Server.Services.Cart;

public interface ICartService
{
    Task<Reply<Cart_DTO?>> PostCartItems( int userId, List<CartItemId_DTO> cartItemsDtos );
    Task<Reply<bool>> AddToCart( int userId, CartItemId_DTO cartItem );
    Task<Reply<bool>> UpdateQuantity( int userId, CartItemId_DTO cartItem );
    Task<Reply<bool>> RemoveFromCart( int userId, CartItemId_DTO cartItem );
    Task<Reply<Cart_DTO?>> GetCartProducts( int userId );
    Task<Reply<int>> CountCartItems( int userId );
}