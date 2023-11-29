using BlazorElectronics.Shared.Cart;

namespace BlazorElectronics.Server.Services.Cart;

public interface ICartService
{
    Task<ApiReply<CartResponse?>> GetCart( int userId );
    Task<ApiReply<CartResponse?>> UpdateCart( int userId, CartRequest request );
    Task<ApiReply<CartResponse?>> AddToCart( int userId, CartItemDto item );
    Task<ApiReply<CartResponse?>> UpdateQuantity( int userId, CartItemDto item );
    Task<ApiReply<CartResponse?>> RemoveFromCart( int userId, int productId );
    Task<ApiReply<bool>> ClearCart( int userId );
}