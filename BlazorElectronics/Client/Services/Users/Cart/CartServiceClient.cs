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


    public Task<ServiceReply<CartResponse?>> GetCart()
    {
        throw new NotImplementedException();
    }
    public Task<ServiceReply<CartResponse?>> UpdateCart()
    {
        throw new NotImplementedException();
    }
    public Task<ServiceReply<CartResponse?>> AddToCart( CartItemDto item )
    {
        throw new NotImplementedException();
    }
    public Task<ServiceReply<CartResponse?>> UpdateCartQuantity( CartItemDto item )
    {
        throw new NotImplementedException();
    }
    public Task<ServiceReply<CartResponse?>> RemoveFromCart( int productId )
    {
        throw new NotImplementedException();
    }
    public Task<ServiceReply<bool>> ClearCart()
    {
        throw new NotImplementedException();
    }

}