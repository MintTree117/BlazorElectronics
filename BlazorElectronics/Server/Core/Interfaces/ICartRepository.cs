using BlazorElectronics.Shared.Cart;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface ICartRepository
{
    Task<CartDto?> UpdateCart( int userId, List<CartItemDto> items );
    Task<bool> InsertOrUpdateItem( int userId, CartItemDto itemDto );
    Task<bool> DeleteItem( int userId, int productId );
    Task<bool> DeleteCart( int userId );
}