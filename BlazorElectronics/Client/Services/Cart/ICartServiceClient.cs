using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Client.Services.Cart;

public interface ICartServiceClient
{
    event Action<int> OnChange;
    event Action<string?> PostErrorEvent;

    Task PostCartToServer( bool emptyLocalCart );
    Task<Reply<Cart_DTO?>> GetCart();
    Task AddToCart( CartItem_DTO item );
    Task UpdateItemQuantity( CartItem_DTO item );
    Task RemoveFromCart( CartItem_DTO item );
    Task<int> GetCartItemsCount();
}