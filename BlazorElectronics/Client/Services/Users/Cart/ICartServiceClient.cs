using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Cart;

namespace BlazorElectronics.Client.Services.Users.Cart;

public interface ICartServiceClient
{
    event Action<int> OnChange;

    Task<ServiceReply<CartResponse?>> GetCart();
    Task<ServiceReply<CartResponse?>> UpdateCart();
    Task<ServiceReply<CartResponse?>> AddToCart( CartItem item );
    Task<ServiceReply<CartResponse?>> UpdateCartQuantity( CartItem item );
    Task<ServiceReply<CartResponse?>> RemoveFromCart( int productId );
    Task<ServiceReply<bool>> ClearCart();
}