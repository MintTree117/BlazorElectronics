using BlazorElectronics.Shared.Cart;

namespace BlazorElectronics.Server.Repositories.Cart;

public interface ICartRepository
{
    Task<IEnumerable<CartProductResponse>?> GetCart( int userId );
    Task<IEnumerable<CartProductResponse>?> UpdateCart( int userId, CartRequest request );
    Task<IEnumerable<CartProductResponse>?> InsertItem( int userId, CartItemDto item );
    Task<IEnumerable<CartProductResponse>?> UpdateQuantity( int userId, CartItemDto item );
    Task<IEnumerable<CartProductResponse>?> DeleteFromCart( int userId, int productId );
    Task<bool> DeleteCart( int userId );
}