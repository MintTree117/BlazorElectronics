using BlazorElectronics.Shared.Cart;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface ICartService
{
    Task<ServiceReply<CartReplyDto?>> GetCart( int userId );
    Task<ServiceReply<CartReplyDto?>> UpdateCart( int userId, CartRequestDto requestDto );
    Task<ServiceReply<CartReplyDto?>> AddToCart( int userId, CartItemDto itemDto );
    Task<ServiceReply<CartReplyDto?>> UpdateQuantity( int userId, CartItemDto itemDto );
    Task<ServiceReply<CartReplyDto?>> RemoveFromCart( int userId, int productId );
    Task<ServiceReply<bool>> ClearCart( int userId );
}