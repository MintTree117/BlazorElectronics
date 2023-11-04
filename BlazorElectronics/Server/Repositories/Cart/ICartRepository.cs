using BlazorElectronics.Server.Models.Cart;
using BlazorElectronics.Server.Models.Products;

namespace BlazorElectronics.Server.Repositories.Cart;

public interface ICartRepository
{
    Task<int?> CountCartItems( int userId );
    Task<IEnumerable<CartItem>?> GetCartItems( int userId );
    Task<IEnumerable<Product>?> GetCartProducts( List<int> productIds, List<int> variantIds );
    Task<bool> AddCartItems( List<CartItem> items );
    Task<bool> AddCartItem( CartItem item );
    Task<bool> UpdateCartItemQuantity( CartItem item );
    Task<bool> RemoveCartItem( CartItem item );
}