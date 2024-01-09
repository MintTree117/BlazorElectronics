using Blazored.LocalStorage;
using BlazorElectronics.Client.Models;
using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Cart;
using BlazorElectronics.Shared.Promos;

namespace BlazorElectronics.Client.Services.Cart;

public class CartServiceClient : UserServiceClient, ICartServiceClient
{
    const string CART_STORAGE_KEY = "cart";
    
    const string API_ROUTE = "api/cart";
    const string API_ROUTE_UPDATE_CART = $"{API_ROUTE}/update";
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
        item = updateReply.Payload?.Products.Find( p => p.ProductId == productId );

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
        CartModel cart = await GetLocalCart();
        List<CartItemDto> localItems = cart.GetItemsDto();
        ServiceReply<CartDto?> reply = await TryUserPostRequest<CartDto>( API_ROUTE_UPDATE_CART, localItems );

        if ( reply is { Success: true, Payload: not null } )
            cart = new CartModel( reply.Payload );

        await TryStoreLocalCart( cart );
        OnChange?.Invoke( cart.GetCartInfo() );
        return new ServiceReply<CartModel?>( cart );
    }
    public async Task<ServiceReply<CartModel?>> AddOrUpdateItem( CartProductDto product )
    {
        CartModel cart = await GetLocalCart();
        cart.AddItem( product );

        ServiceReply<CartDto?> reply = await TryUserPostRequest<CartDto?>( API_ROUTE_UPDATE_CART, cart.GetItemsDto() );
        
        if ( reply is { Success: true, Payload: not null } )
            cart = new CartModel( reply.Payload );

        await TryStoreLocalCart( cart );
        OnChange?.Invoke( cart.GetCartInfo() );
        return new ServiceReply<CartModel?>( cart );
    }
    public async Task<ServiceReply<CartModel?>> RemoveItem( int productId )
    {
        CartModel cart = await GetLocalCart();
        cart.RemoveItem( productId );

        ServiceReply<CartDto?> reply = await TryUserPostRequest<CartDto?>( API_ROUTE_UPDATE_CART, cart.GetItemsDto() );

        if ( reply is { Success: true, Payload: not null } )
            cart = new CartModel( reply.Payload );

        await TryStoreLocalCart( cart );
        OnChange?.Invoke( cart.GetCartInfo() );
        return new ServiceReply<CartModel?>( cart );
    }
    public async Task<ServiceReply<bool>> ClearCart()
    {
        await TryStoreLocalCart( new CartModel() );
        OnChange?.Invoke( new CartInfoModel() );
        return await TryUserPostRequest<bool>( API_ROUTE_UPDATE_CART, new List<CartItemDto>() );
    }
    
    public async Task<ServiceReply<PromoCodeDto?>> AddPromoCode( string code )
    {
        ServiceReply<PromoCodeDto?> reply = await TryUserPutRequest<PromoCodeDto>( API_ROUTE_ADD_PROMO, code );

        if ( !reply.Success || reply.Payload is null )
            return new ServiceReply<PromoCodeDto?>( reply.ErrorType, reply.Message );

        CartModel cart = await GetLocalCart();
        cart.AddPromo( reply.Payload );
        await TryStoreLocalCart( cart );
        
        OnChange?.Invoke( cart.GetCartInfo() );
        return new ServiceReply<PromoCodeDto?>( reply.Payload );
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

    async Task TryStoreLocalCart( CartModel cart )
    {
        try
        {
            await Storage.SetItemAsync( CART_STORAGE_KEY, cart );
        }
        catch ( Exception e )
        {
            Logger.LogError( e.Message, e );
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