using System.Net.Http.Json;
using Blazored.LocalStorage;
using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Inbound.Cart;
using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Client.Services.Cart;

public class CartServiceClient : ICartServiceClient
{
    public event Action<int>? OnChange;
    public event Action<string>? PostErrorEvent;

    const string CART_STORAGE_KEY = "cart";
    const string POST_ERROR_DEFAULT_MESSAGE = "Cart post error default message!";

    readonly IUserServiceClient _userService;
    readonly ILocalStorageService _localStorage;
    readonly HttpClient _http;

    public CartServiceClient( IUserServiceClient userService, ILocalStorageService localStorage, HttpClient http )
    {
        _userService = userService;
        _localStorage = localStorage;
        _http = http;
    }
    
    public async Task PostCartToServer( bool emptyLocalCart = false )
    {
        Cart_DTO localCart = await GetCartFromStorage();

        if ( localCart == null )
            return;

        ServiceResponse<bool> authorizeResponse = await _userService.AuthorizeUser();

        if ( !authorizeResponse.Data )
            return;

        CartItemsInsertRequest cartIds = GetCartIds( localCart );

        try
        {
            HttpResponseMessage httpResponse = await _http.PostAsJsonAsync( "api/Cart/post", cartIds );
            var serviceResponse = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<Cart_DTO?>>();

            if ( serviceResponse == null )
            {
                PostErrorEvent?.Invoke( "Post response is null!" );
                return;
            }
            if ( !serviceResponse.Success )
            {
                PostErrorEvent?.Invoke( serviceResponse.Message ??= POST_ERROR_DEFAULT_MESSAGE );
                return;
            }

            await _localStorage.SetItemAsync( CART_STORAGE_KEY, serviceResponse.Data );
        }
        catch ( Exception e )
        {
            PostErrorEvent?.Invoke( e.Message );
        }
    }
    public async Task<ServiceResponse<Cart_DTO?>> GetCart()
    {
        ServiceResponse<bool> authorizeResponse = await _userService.AuthorizeUser();

        if ( !authorizeResponse.Data )
        {
            Cart_DTO localCart = await GetCartFromStorage();
            return new ServiceResponse<Cart_DTO?>( localCart, true, "Got cart from local storage." );
        }

        ServiceResponse<Cart_DTO?> serverResponse = await GetCartFromServer();

        if ( !serverResponse.Success )
            return serverResponse;

        await _localStorage.SetItemAsync( CART_STORAGE_KEY, serverResponse.Data );
        return serverResponse;
    }
    public async Task AddToCart( CartItem_DTO item )
    {
        Cart_DTO cart = await GetCartFromStorage();
        cart.AddOrUpdateQuantity( item );
        await _localStorage.SetItemAsync( CART_STORAGE_KEY, cart );

        ServiceResponse<bool> authorizeResponse = await _userService.AuthorizeUser();

        if ( !authorizeResponse.Data )
        {
            OnChange?.Invoke( cart.Items.Count );
            return;
        }

        try
        {
            var dto = new CartItemId_DTO {
                ProductId = item.ProductId,
                VariantId = item.VariantId,
                Quantity = item.Quantity
            };
            HttpResponseMessage httpResponse = await _http.PostAsJsonAsync( "api/Cart/insert", dto );
            var serviceResponse = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<bool>>();

            if ( serviceResponse == null )
            {
                PostErrorEvent?.Invoke( "Post response is null!" );
                throw new Exception();
                return;
            }
            if ( !serviceResponse.Success )
            {
                PostErrorEvent?.Invoke( serviceResponse.Message ??= POST_ERROR_DEFAULT_MESSAGE );
                return;
            }
            
            OnChange?.Invoke( await GetCartItemsCount() );
        }
        catch ( Exception e )
        {
            PostErrorEvent?.Invoke( e.Message );
        }
    }
    public async Task RemoveFromCart( CartItem_DTO item )
    {
        Cart_DTO cart = await GetCartFromStorage();

        if ( cart.GetSameItem( item, out CartItem_DTO? sameItem ) )
            cart.Items.Remove( sameItem! );
        
        await _localStorage.SetItemAsync( CART_STORAGE_KEY, cart );

        ServiceResponse<bool> authorizeResponse = await _userService.AuthorizeUser();

        if ( !authorizeResponse.Data )
        {
            OnChange?.Invoke( cart.Items.Count );
            return;
        }

        try
        {
            var dto = new CartItemId_DTO {
                ProductId = item.ProductId,
                VariantId = item.VariantId,
                Quantity = item.Quantity
            };
            HttpResponseMessage httpResponse = await _http.PostAsJsonAsync( "api/Cart/remove", dto );
            var serviceResponse = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<bool>>();

            if ( serviceResponse == null )
            {
                PostErrorEvent?.Invoke( "Post response is null!" );
                return;
            }
            if ( !serviceResponse.Success )
            {
                PostErrorEvent?.Invoke( serviceResponse.Message ??= POST_ERROR_DEFAULT_MESSAGE );
                return;
            }

            OnChange?.Invoke( await GetCartItemsCount() );
        }
        catch ( Exception e )
        {
            PostErrorEvent?.Invoke( e.Message );
        }
    }
    public async Task UpdateItemQuantity( CartItem_DTO item )
    {
        Cart_DTO cart = await GetCartFromStorage();

        if ( !cart.GetSameItem( item, out CartItem_DTO? sameItem ) )
            return;

        sameItem!.Quantity = item.Quantity;
        await _localStorage.SetItemAsync( CART_STORAGE_KEY, cart );

        ServiceResponse<bool> authorizeResponse = await _userService.AuthorizeUser();

        if ( !authorizeResponse.Data )
            return;

        try
        {
            var dto = new CartItemId_DTO {
                ProductId = item.ProductId,
                VariantId = item.VariantId,
                Quantity = item.Quantity
            };
            
            HttpResponseMessage httpResponse = await _http.PostAsJsonAsync( "api/Cart/update-quantity", dto );
            var serviceResponse = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<bool>>();

            if ( serviceResponse == null )
            {
                PostErrorEvent?.Invoke( "Post response is null!" );
                return;
            }
            if ( !serviceResponse.Success )
                PostErrorEvent?.Invoke( serviceResponse.Message ??= POST_ERROR_DEFAULT_MESSAGE );
        }
        catch ( Exception e )
        {
            PostErrorEvent?.Invoke( e.Message );
        }
    }
    public async Task<int> GetCartItemsCount()
    {
        ServiceResponse<bool> authorizeResponse = await _userService.AuthorizeUser();
        
        if ( !authorizeResponse.Data )
        {
            var cart = await _localStorage.GetItemAsync<Cart_DTO>( "cart" );
            return cart != null ? cart.Items.Count : 0;
        }

        try
        {
            var response = await _http.GetFromJsonAsync<ServiceResponse<int>>( "api/Cart/count" );
            return response is not { Success: true } 
                ? 0 
                : response.Data;
        }
        catch ( Exception e )
        {
            PostErrorEvent?.Invoke( e.Message );
            return 0;
        }
    }
    
    async Task<ServiceResponse<Cart_DTO?>> GetCartFromServer()
    {
        try
        {
            var response = await _http.GetFromJsonAsync<ServiceResponse<Cart_DTO?>>( "api/Cart/products" );

            if ( response == null )
                return new ServiceResponse<Cart_DTO?>( null, false, "Service response is null!" );

            return !response.Success
                ? new ServiceResponse<Cart_DTO?>( null, false, response.Message ??= "Failed to retrieve Cart; message is null!" )
                : response;
        }
        catch ( Exception e )
        {
            return new ServiceResponse<Cart_DTO?>( null, false, e.Message );
        }
    }
    async Task<Cart_DTO> GetCartFromStorage()
    {
        return await _localStorage.GetItemAsync<Cart_DTO>( CART_STORAGE_KEY ) ?? new Cart_DTO();
    }
    static CartItemsInsertRequest GetCartIds( Cart_DTO cart )
    {
        var cartIds = new CartItemsInsertRequest();

        foreach ( CartItem_DTO item in cart.Items )
        {
            cartIds.Items.Add( new CartItemId_DTO {
                ProductId = item.ProductId,
                VariantId = item.VariantId,
                Quantity = item.Quantity
            } );
        }

        return cartIds;
    }
}