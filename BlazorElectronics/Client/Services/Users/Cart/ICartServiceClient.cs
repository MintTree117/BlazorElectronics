using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Cart;

namespace BlazorElectronics.Client.Services.Users.Cart;

public interface ICartServiceClient
{
    event Action<int> OnChange;

    Task<ServiceReply<CartResponse?>> GetCart();
    Task<ServiceReply<CartResponse?>> UpdateCart();
    Task<ServiceReply<CartResponse?>> AddToCart( CartItemDto item );
    Task<ServiceReply<CartResponse?>> UpdateCartQuantity( CartItemDto item );
    Task<ServiceReply<CartResponse?>> RemoveFromCart( int productId );
    Task<ServiceReply<bool>> ClearCart();
}