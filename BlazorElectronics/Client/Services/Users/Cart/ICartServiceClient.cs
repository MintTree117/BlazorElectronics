using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Client.Services.Users.Cart;

public interface ICartServiceClient
{
    event Action<int> OnChange;
    event Action<string?> PostErrorEvent;

    Task PostCartToServer( bool emptyLocalCart );
    Task<ApiReply<CartResponse?>> GetCart();
    Task AddToCart( CartProductResponse item );
    Task UpdateItemQuantity( CartProductResponse item );
    Task RemoveFromCart( CartProductResponse item );
    Task<int> GetCartItemsCount();
}