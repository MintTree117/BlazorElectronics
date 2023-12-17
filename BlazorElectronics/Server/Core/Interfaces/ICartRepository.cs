using BlazorElectronics.Shared.Cart;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface ICartRepository
{
    Task<IEnumerable<CartProductDto>?> GetCart( int userId );
    Task<IEnumerable<CartProductDto>?> UpdateCart( int userId, CartRequestDto requestDto );
    Task<IEnumerable<CartProductDto>?> InsertItem( int userId, CartItemDto itemDto );
    Task<IEnumerable<CartProductDto>?> UpdateQuantity( int userId, CartItemDto itemDto );
    Task<IEnumerable<CartProductDto>?> DeleteFromCart( int userId, int productId );
    Task<bool> DeleteCart( int userId );
}