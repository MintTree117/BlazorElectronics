using BlazorElectronics.Shared.Cart;
using BlazorElectronics.Shared.Promos;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface ICartRepository
{
    Task<CartDto?> Get( int userId );
    Task<CartDto?> Update( int userId, List<CartItemDto> items );
    Task<bool> InsertOrUpdateItem( int userId, CartItemDto itemDto );
    Task<bool> DeleteItem( int userId, int productId );
    Task<bool> DeleteCart( int userId );

    Task<PromoCodeDto?> InsertCartPromo( int userId, string code );
    Task<bool> DeleteCartPromo( int userId, int promoId );
}