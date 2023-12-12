using BlazorElectronics.Shared.Cart;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface ICartRepository
{
    Task<IEnumerable<CartProductResponse>?> GetCart( int userId );
    Task<IEnumerable<CartProductResponse>?> UpdateCart( int userId, CartRequest request );
    Task<IEnumerable<CartProductResponse>?> InsertItem( int userId, CartItem item );
    Task<IEnumerable<CartProductResponse>?> UpdateQuantity( int userId, CartItem item );
    Task<IEnumerable<CartProductResponse>?> DeleteFromCart( int userId, int productId );
    Task<bool> DeleteCart( int userId );
}