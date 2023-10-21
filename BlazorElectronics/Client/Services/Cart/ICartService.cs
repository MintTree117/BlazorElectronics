using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Client.Services.Cart;

public interface ICartService
{
    event Action<int> OnChange;

    Task AddToCart( CartItem_DTO item );
    Task UpdateItemQuantity( CartItem_DTO item );
    Task<Cart_DTO> GetItemsFromLocalStorage();
    Task<Cart_DTO?> GetItemsFromServer();
    Task RemoveItemFromCart( int productId, int variantId );
}