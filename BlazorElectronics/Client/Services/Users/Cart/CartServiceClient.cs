using System.Net.Http.Json;
using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Inbound.Cart;
using BlazorElectronics.Shared.Mutual;
using BlazorElectronics.Shared.Outbound.Users;

namespace BlazorElectronics.Client.Services.Users.Cart;

public class CartServiceClient : UserServiceClient, ICartServiceClient
{
    public event Action<int>? OnChange;
    public event Action<string>? PostErrorEvent;

    const string CART_STORAGE_KEY = "cart";
    const string POST_ERROR_DEFAULT_MESSAGE = "Cart post error default message!";

    public CartServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }

    public async Task PostCartToServer( bool emptyLocalCart = false )
    {
        CartResponse localCart = await GetCartFromStorage();

        if ( localCart == null )
            return;

        ApiReply<UserSessionResponse?> authorizeResponse = await AuthorizeUser();

        if ( !authorizeResponse.Success || authorizeResponse.Data is null )
            return;

        CartItemsInsertRequest cartIds = GetCartIds( localCart );

        try
        {
            HttpResponseMessage httpResponse = await Http.PostAsJsonAsync( "api/Cart/post", cartIds );
            var serviceResponse = await httpResponse.Content.ReadFromJsonAsync<ApiReply<CartResponse?>>();

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

            await Storage.SetItemAsync( CART_STORAGE_KEY, serviceResponse.Data );
        }
        catch ( Exception e )
        {
            PostErrorEvent?.Invoke( e.Message );
        }
    }
    public async Task<ApiReply<CartResponse?>> GetCart()
    {
        ApiReply<UserSessionResponse?> authorizeResponse = await AuthorizeUser();

        if ( !authorizeResponse.Success || authorizeResponse.Data is null )
        {
            CartResponse localCart = await GetCartFromStorage();
            return new ApiReply<CartResponse?>( localCart, true, "Got cart from local storage." );
        }

        ApiReply<CartResponse?> serverResponse = await GetCartFromServer();

        if ( !serverResponse.Success )
            return serverResponse;

        await Storage.SetItemAsync( CART_STORAGE_KEY, serverResponse.Data );
        return serverResponse;
    }
    public async Task AddToCart( CartProductResponse item )
    {
        CartResponse cart = await GetCartFromStorage();
        cart.AddOrUpdateQuantity( item );
        await Storage.SetItemAsync( CART_STORAGE_KEY, cart );

        ApiReply<UserSessionResponse?> authorizeResponse = await AuthorizeUser();

        if ( !authorizeResponse.Success || authorizeResponse.Data is null )
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
            HttpResponseMessage httpResponse = await Http.PostAsJsonAsync( "api/Cart/insert", dto );
            var serviceResponse = await httpResponse.Content.ReadFromJsonAsync<ApiReply<bool>>();

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
    public async Task RemoveFromCart( CartProductResponse item )
    {
        CartResponse cart = await GetCartFromStorage();

        if ( cart.GetSameItem( item, out CartProductResponse? sameItem ) )
            cart.Items.Remove( sameItem! );
        
        await Storage.SetItemAsync( CART_STORAGE_KEY, cart );

        ApiReply<UserSessionResponse?> authorizeResponse = await AuthorizeUser();

        if ( !authorizeResponse.Success || authorizeResponse.Data is null )
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
            HttpResponseMessage httpResponse = await Http.PostAsJsonAsync( "api/Cart/remove", dto );
            var serviceResponse = await httpResponse.Content.ReadFromJsonAsync<ApiReply<bool>>();

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
    public async Task UpdateItemQuantity( CartProductResponse item )
    {
        CartResponse cart = await GetCartFromStorage();

        if ( !cart.GetSameItem( item, out CartProductResponse? sameItem ) )
            return;

        sameItem!.Quantity = item.Quantity;
        await Storage.SetItemAsync( CART_STORAGE_KEY, cart );

        ApiReply<UserSessionResponse?> authorizeResponse = await AuthorizeUser();

        if ( !authorizeResponse.Success || authorizeResponse.Data is null )
            return;

        try
        {
            var dto = new CartItemId_DTO {
                ProductId = item.ProductId,
                VariantId = item.VariantId,
                Quantity = item.Quantity
            };
            
            HttpResponseMessage httpResponse = await Http.PostAsJsonAsync( "api/Cart/update-quantity", dto );
            var serviceResponse = await httpResponse.Content.ReadFromJsonAsync<ApiReply<bool>>();

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
        /*ApiReply<bool> authorizeResponse = await _userService.AuthorizeUser();
        
        if ( !authorizeResponse.Data )
        {
            var cart = await _localStorage.GetItemAsync<CartResponse>( "cart" );
            return cart != null ? cart.Items.Count : 0;
        }

        try
        {
            var response = await _http.GetFromJsonAsync<ApiReply<int>>( "api/Cart/count" );
            return response is not { Success: true } 
                ? 0 
                : response.Data;
        }
        catch ( Exception e )
        {
            PostErrorEvent?.Invoke( e.Message );
            return 0;
        }*/
        return 0;
    }
    
    async Task<ApiReply<CartResponse?>> GetCartFromServer()
    {
        try
        {
            var response = await Http.GetFromJsonAsync<ApiReply<CartResponse?>>( "api/Cart/products" );

            if ( response == null )
                return new ApiReply<CartResponse?>( null, false, "Service response is null!" );

            return !response.Success
                ? new ApiReply<CartResponse?>( null, false, response.Message ??= "Failed to retrieve Cart; message is null!" )
                : response;
        }
        catch ( Exception e )
        {
            return new ApiReply<CartResponse?>( null, false, e.Message );
        }
    }
    async Task<CartResponse> GetCartFromStorage()
    {
        return await Storage.GetItemAsync<CartResponse>( CART_STORAGE_KEY ) ?? new CartResponse();
    }
    static CartItemsInsertRequest GetCartIds( CartResponse cart )
    {
        var cartIds = new CartItemsInsertRequest();

        foreach ( CartProductResponse item in cart.Items )
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