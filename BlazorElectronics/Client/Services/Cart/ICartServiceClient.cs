using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Cart;

namespace BlazorElectronics.Client.Services.Cart;

public interface ICartServiceClient
{
    event Action<int> OnChange;

    Task<ServiceReply<CartReplyDto?>> GetCart();
    Task<ServiceReply<bool>> UpdateCart();
    Task<ServiceReply<bool>> AddToCart( CartItemDto itemDto );
    Task<ServiceReply<bool>> UpdateCartQuantity( CartItemDto itemDto );
    Task<ServiceReply<bool>> RemoveFromCart( int productId );
    Task<ServiceReply<bool>> ClearCart();
}