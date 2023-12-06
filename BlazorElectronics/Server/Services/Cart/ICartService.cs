using BlazorElectronics.Shared.Cart;

namespace BlazorElectronics.Server.Services.Cart;

public interface ICartService
{
    Task<ServiceReply<CartResponse?>> GetCart( int userId );
    Task<ServiceReply<CartResponse?>> UpdateCart( int userId, CartRequest request );
    Task<ServiceReply<CartResponse?>> AddToCart( int userId, CartItem item );
    Task<ServiceReply<CartResponse?>> UpdateQuantity( int userId, CartItem item );
    Task<ServiceReply<CartResponse?>> RemoveFromCart( int userId, int productId );
    Task<ServiceReply<bool>> ClearCart( int userId );
}