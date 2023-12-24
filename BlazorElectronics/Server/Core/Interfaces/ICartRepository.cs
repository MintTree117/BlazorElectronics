using BlazorElectronics.Shared.Cart;
using BlazorElectronics.Shared.Promos;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface ICartRepository
{
    Task<CartDto?> GetCart( int userId );
    Task<CartDto?> UpdateCart( int userId, List<CartItemDto> items );
    Task<bool> InsertOrUpdateItem( int userId, CartItemDto itemDto );
    Task<bool> DeleteItem( int userId, int productId );
    Task<bool> DeleteCart( int userId );

    Task<PromoCodeDto?> InsertCartPromo( int userId, string code );
    Task<bool> DeleteCartPromo( int userId, int promoId );
}