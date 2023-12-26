using Blazored.LocalStorage;
using BlazorElectronics.Client.Models;
using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Cart;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Promos;

namespace BlazorElectronics.Client.Services.Cart;

public class CartServiceClient : UserServiceClient, ICartServiceClient
{
    const string CART_STORAGE_KEY = "cart";
    
    const string API_ROUTE = "api/cart";
    const string API_ROUTE_UPDATE_CART = $"{API_ROUTE}/update-cart";
    const string API_ROUTE_ADD_ITEM = $"{API_ROUTE}/add-update-item";
    const string API_ROUTE_REMOVE_ITEM = $"{API_ROUTE}/remove-item";
    const string API_ROUTE_CLEAR_CART = $"{API_ROUTE}/clear-cart";
    const string API_ROUTE_ADD_PROMO = $"{API_ROUTE}/add-promo";
    const string API_ROUTE_REMOVE_PROMO = $"{API_ROUTE}/remove-promo";
    
    public CartServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }

    public event Action<CartInfoModel>? OnChange;

    public async Task<ServiceReply<int>> HasItem( int productId )
    {
        CartModel cart = await GetLocalCart();
        CartProductDto? item = cart.Products.Find( p => p.ProductId == productId );

        if ( item != null )
            return new ServiceReply<int>( item.Quantity );

        ServiceReply<CartModel?> updateReply = await UpdateCart();
        item = updateReply.Data?.Products.Find( p => p.ProductId == productId );

        return item is not null
            ? new ServiceReply<int>( item.Quantity )
            : new ServiceReply<int>( updateReply.ErrorType, updateReply.Message );
    }
    public async Task<ServiceReply<CartInfoModel>> GetLocalCartInfo()
    {
        CartModel cart = await GetLocalCart();
        return new ServiceReply<CartInfoModel>( cart.GetCartInfo() );
    }
    public async Task<ServiceReply<CartModel?>> UpdateCart()
    {
        List<CartItemDto> localItems = ( await GetLocalCart() ).GetItemsDto();
        ServiceReply<CartDto?> reply = await TryUserPostRequest<CartDto>( API_ROUTE_UPDATE_CART, localItems );

        if ( reply is { Success: true, Data: not null } )
        {
            CartModel cart = new( reply.Data );
            await TryStoreLocalCart( cart );
            OnChange?.Invoke( cart.GetCartInfo() );
            return new ServiceReply<CartModel?>( cart );
        }
        
        OnChange?.Invoke( new CartInfoModel() );
        return new ServiceReply<CartModel?>( new CartModel() );
    }
    public async Task<ServiceReply<bool>> AddOrUpdateItem( CartProductDto product )
    {
        CartItemDto dto = new( product.ProductId, product.Quantity );
        ServiceReply<bool> reply = await TryUserPostRequest<bool>( API_ROUTE_ADD_ITEM, dto );
        
        CartModel cart = await GetLocalCart();
        cart.AddItem( product );
        bool storedLocally = await TryStoreLocalCart( cart );

        if ( !reply.Success && !storedLocally )
            return new ServiceReply<bool>( ServiceErrorType.NotFound, "Failed to add item to local storage or server!" );
        
        OnChange?.Invoke( cart.GetCartInfo() );
        return new ServiceReply<bool>( true );
    }
    public async Task<ServiceReply<bool>> RemoveItem( int productId )
    {
        ServiceReply<bool> reply = await TryUserDeleteRequest<bool>( API_ROUTE_REMOVE_ITEM, GetIdParam( productId ) );
        
        CartModel cart = await GetLocalCart();
        cart.RemoveItem( productId );
        bool storedLocally = await TryStoreLocalCart( cart );

        if ( !reply.Success && !storedLocally )
            return new ServiceReply<bool>( ServiceErrorType.NotFound, "Failed to remove item from local storage or server!" );

        OnChange?.Invoke( cart.GetCartInfo() );
        return new ServiceReply<bool>( true );
    }
    public async Task<ServiceReply<bool>> ClearCart()
    {
        await TryStoreLocalCart( new CartModel() );
        return await TryUserDeleteRequest<bool>( API_ROUTE_CLEAR_CART );
    }
    
    public async Task<ServiceReply<PromoCodeDto?>> AddPromoCode( string code )
    {
        ServiceReply<PromoCodeDto?> reply = await TryUserPutRequest<PromoCodeDto>( API_ROUTE_ADD_PROMO, code );

        if ( !reply.Success || reply.Data is null )
            return new ServiceReply<PromoCodeDto?>( reply.ErrorType, reply.Message );

        CartModel cart = await GetLocalCart();
        cart.AddPromo( reply.Data );
        await TryStoreLocalCart( cart );
        
        OnChange?.Invoke( cart.GetCartInfo() );
        return new ServiceReply<PromoCodeDto?>( reply.Data );
    }
    public async Task<ServiceReply<bool>> RemovePromoCode( int id )
    {
        CartModel cart = await GetLocalCart();
        cart.RemovePromo( id );
        await TryStoreLocalCart( cart );

        OnChange?.Invoke( cart.GetCartInfo() );
        
        ServiceReply<bool> reply = await TryUserDeleteRequest<bool>( API_ROUTE_REMOVE_PROMO, GetRemovePromoParam( id ) );

        return reply.Success
            ? new ServiceReply<bool>( true )
            : new ServiceReply<bool>( reply.ErrorType, reply.Message );
    }

    async Task<bool> TryStoreLocalCart( CartModel cart )
    {
        try
        {
            await Storage.SetItemAsync( CART_STORAGE_KEY, cart );
            return true;
        }
        catch ( Exception e )
        {
            Logger.LogError( e.Message, e );
            return false;
        }
    }
    async Task<CartModel> GetLocalCart()
    {
        try
        {
            var cart = await Storage.GetItemAsync<CartModel>( CART_STORAGE_KEY );
            return cart ?? new CartModel();
        }
        catch ( Exception e )
        {
            Logger.LogError( e.Message, e );
            return new CartModel();
        }
    }
    
    static Dictionary<string, object> GetRemovePromoParam( int id )
    {
        return new Dictionary<string, object>
        {
            { "PromoId", id }
        };
    }
}