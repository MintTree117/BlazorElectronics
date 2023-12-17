using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Cart;

namespace BlazorElectronics.Client.Services.Users.Cart;

public class CartServiceClient : UserServiceClient, ICartServiceClient
{
    public event Action<int>? OnChange;
    const string CART_STORAGE_KEY = "cart";

    public CartServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }


    public Task<ServiceReply<CartReplyDto?>> GetCart()
    {
        throw new NotImplementedException();
    }
    public Task<ServiceReply<CartReplyDto?>> UpdateCart()
    {
        throw new NotImplementedException();
    }
    public Task<ServiceReply<CartReplyDto?>> AddToCart( CartItemDto itemDto )
    {
        throw new NotImplementedException();
    }
    public Task<ServiceReply<CartReplyDto?>> UpdateCartQuantity( CartItemDto itemDto )
    {
        throw new NotImplementedException();
    }
    public Task<ServiceReply<CartReplyDto?>> RemoveFromCart( int productId )
    {
        throw new NotImplementedException();
    }
    public Task<ServiceReply<bool>> ClearCart()
    {
        throw new NotImplementedException();
    }

}