using BlazorElectronics.Shared.Cart;
using BlazorElectronics.Shared.Promos;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface ICartService
{
    Task<ServiceReply<CartDto?>> UpdateCart( int userId, List<CartItemDto> items );
    Task<ServiceReply<bool>> AddToCart( int userId, CartItemDto itemDto );
    Task<ServiceReply<bool>> RemoveFromCart( int userId, int productId );
    Task<ServiceReply<bool>> ClearCart( int userId );
    Task<ServiceReply<PromoCodeDto?>> AddPromo( int userId, string code );
    Task<ServiceReply<bool>> RemovePromo( int userId, int promoId );
}