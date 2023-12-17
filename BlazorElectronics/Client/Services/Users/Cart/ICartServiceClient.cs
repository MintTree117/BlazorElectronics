using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Cart;

namespace BlazorElectronics.Client.Services.Users.Cart;

public interface ICartServiceClient
{
    event Action<int> OnChange;

    Task<ServiceReply<CartReplyDto?>> GetCart();
    Task<ServiceReply<CartReplyDto?>> UpdateCart();
    Task<ServiceReply<CartReplyDto?>> AddToCart( CartItemDto itemDto );
    Task<ServiceReply<CartReplyDto?>> UpdateCartQuantity( CartItemDto itemDto );
    Task<ServiceReply<CartReplyDto?>> RemoveFromCart( int productId );
    Task<ServiceReply<bool>> ClearCart();
}