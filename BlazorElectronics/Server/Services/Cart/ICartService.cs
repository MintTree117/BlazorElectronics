using BlazorElectronics.Shared.Inbound.Cart;
using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Server.Services.Cart;

public interface ICartService
{
    Task<ServiceResponse<Cart_DTO?>> PostCartItems( int userId, List<CartItemId_DTO> cartItemsDtos );
    Task<ServiceResponse<bool>> AddToCart( int userId, CartItemId_DTO cartItem );
    Task<ServiceResponse<bool>> UpdateQuantity( int userId, CartItemId_DTO cartItem );
    Task<ServiceResponse<bool>> RemoveFromCart( int userId, CartItemId_DTO cartItem );
    Task<ServiceResponse<Cart_DTO?>> GetCartProducts( int userId );
    Task<ServiceResponse<int>> CountCartItems( int userId );
}