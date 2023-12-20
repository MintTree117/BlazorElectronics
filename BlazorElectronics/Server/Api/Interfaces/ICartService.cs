using BlazorElectronics.Shared.Cart;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface ICartService
{
    Task<ServiceReply<CartDto?>> UpdateCart( int userId, List<CartItemDto> items );
    Task<ServiceReply<bool>> AddToCart( int userId, CartItemDto itemDto );
    Task<ServiceReply<bool>> RemoveFromCart( int userId, int productId );
    Task<ServiceReply<bool>> ClearCart( int userId );
}