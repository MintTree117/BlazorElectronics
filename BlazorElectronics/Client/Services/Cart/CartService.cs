using System.Net.Http.Json;
using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Inbound.Cart;
using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Client.Services.Cart;

public class CartService : ICartService
{
    public event Action? OnChange;

    const string CART_STORAGE_KEY = "cart";
    readonly ILocalStorageService _localStorage;
    readonly HttpClient _http;

    public CartService( ILocalStorageService localStorage, HttpClient http )
    {
        _localStorage = localStorage;
        _http = http;
    }
    
    public async Task AddToCart( CartItem_DTO item )
    {
        Cart_DTO cart = await GetCartFromStorage();
        cart.Items.Add( item );
        await _localStorage.SetItemAsync( CART_STORAGE_KEY, cart );
        OnChange?.Invoke();
    }
    public async Task<Cart_DTO> GetItemsFromLocalStorage()
    {
        return await GetCartFromStorage();
    }
    public async Task<Cart_DTO?> GetItemsFromServer()
    {
        Cart_DTO cart = await GetCartFromStorage();

        if ( cart.Items.Count <= 0 )
            return cart;

        var cartIds = new CartItemIds();
        
        foreach ( CartItem_DTO item in cart.Items )
        {
            cartIds.Items.Add( new CartItemId {
                ProductId = item.ProductId,
                VariantId = item.VariantId
            } );
        }
        
        HttpResponseMessage response = await _http.PostAsJsonAsync( "api/Cart/items", cartIds );
        var serverCart = await response.Content.ReadFromJsonAsync<ServiceResponse<Cart_DTO?>?>();
        return serverCart?.Data;
    }

    async Task<Cart_DTO> GetCartFromStorage()
    {
        return new Cart_DTO {
            Items = await _localStorage.GetItemAsync<List<CartItem_DTO>>( CART_STORAGE_KEY ) ?? new List<CartItem_DTO>()
        };
    }
}