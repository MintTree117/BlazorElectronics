using BlazorElectronics.Shared.Cart;

namespace BlazorElectronics.Server.Services.Cart;

public interface ICartService
{
    Task<ApiReply<CartResponse?>> PostCartItems( int userId, List<CartItemIdsDto> cartItemsDtos );
    Task<ApiReply<bool>> AddToCart( int userId, CartItemIdsDto cartItem );
    Task<ApiReply<bool>> UpdateQuantity( int userId, CartItemIdsDto cartItem );
    Task<ApiReply<bool>> RemoveFromCart( int userId, CartItemIdsDto cartItem );
    Task<ApiReply<CartResponse?>> GetCartProducts( int userId );
    Task<ApiReply<int>> CountCartItems( int userId );
}