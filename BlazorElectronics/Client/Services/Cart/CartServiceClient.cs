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
    
    public async Task<ServiceReply<CartInfoModel>> GetLocalCartInfo()
    {
        CartModel cart = await GetLocalCart();
        return new ServiceReply<CartInfoModel>( cart.GetCartInfo() );
    }
    public async Task<ServiceReply<CartModel?>> UpdateCart()
    {
        List<CartItemDto> localItems = ( await GetLocalCart() ).GetItemsDto();
        ServiceReply<CartDto?> reply = await TryUserRequest<List<CartItemDto>, CartDto>( API_ROUTE_UPDATE_CART, localItems );

        if ( !reply.Success || reply.Data is null )
            return new ServiceReply<CartModel?>( reply.ErrorType, $"Failed to update cart! {reply.Message}" );

        CartModel cart = new( reply.Data );
        
        await TryStoreLocalCart( cart );
        OnChange?.Invoke( cart.GetCartInfo() );
        
        return new ServiceReply<CartModel?>( cart );
    }
    public async Task<ServiceReply<int>> HasItem( int productId )
    {
        CartModel cart = await GetLocalCart();
        CartProductDto? item = cart.Products.Find( p => p.ProductId == productId );

        if ( item != null )
            return new ServiceReply<int>( item.ItemQuantity );

        ServiceReply<CartModel?> updateReply = await UpdateCart();
        item = updateReply.Data?.Products.Find( p => p.ProductId == productId );

        return item is not null
            ? new ServiceReply<int>( item.ItemQuantity )
            : new ServiceReply<int>( updateReply.ErrorType, updateReply.Message );
    }
    public async Task<ServiceReply<bool>> AddOrUpdateItem( CartProductDto product )
    {
        CartItemDto dto = new( product.ProductId, product.ItemQuantity );
        ServiceReply<bool> reply = await TryUserRequest<CartItemDto, bool>( API_ROUTE_ADD_ITEM, dto );
        
        CartModel cart = await GetLocalCart();
        cart.AddItem( product );
        bool storedLocally = await TryStoreLocalCart( cart );

        if ( !reply.Success || !storedLocally )
            return new ServiceReply<bool>( ServiceErrorType.NotFound, "Failed to add item to local storage or server!" );
        
        OnChange?.Invoke( cart.GetCartInfo() );
        return new ServiceReply<bool>( true );
    }
    public async Task<ServiceReply<bool>> RemoveItem( int productId )
    {
        IntDto intDto = new( productId );
        ServiceReply<bool> reply = await TryUserRequest<IntDto, bool>( API_ROUTE_REMOVE_ITEM, intDto );
        
        CartModel cart = await GetLocalCart();
        cart.RemoveItem( productId );
        bool storedLocally = await TryStoreLocalCart( cart );

        if ( !reply.Success || !storedLocally )
            return new ServiceReply<bool>( ServiceErrorType.NotFound, "Failed to remove item from local storage or server!" );

        OnChange?.Invoke( cart.GetCartInfo() );
        return new ServiceReply<bool>( true );
    }
    public async Task<ServiceReply<bool>> ClearCart()
    {
        await TryStoreLocalCart( new CartModel() );
        return await TryUserRequest<bool>( API_ROUTE_CLEAR_CART );
    }
    
    public async Task<ServiceReply<bool>> AddPromoCode( string code )
    {
        ServiceReply<PromoCodeDto?> reply = await TryUserRequest<string, PromoCodeDto>( API_ROUTE_ADD_PROMO, code );

        if ( !reply.Success || reply.Data is null )
            return new ServiceReply<bool>( reply.ErrorType, reply.Message );

        CartModel cart = await GetLocalCart();
        cart.AddPromo( reply.Data );
        await TryStoreLocalCart( cart );
        
        OnChange?.Invoke( cart.GetCartInfo() );
        return new ServiceReply<bool>( true );
    }
    public async Task<ServiceReply<bool>> RemovePromoCode( string code )
    {
        CartModel cart = await GetLocalCart();
        cart.RemovePromo( code );
        await TryStoreLocalCart( cart );

        OnChange?.Invoke( cart.GetCartInfo() );
        
        ServiceReply<bool> reply = await TryUserRequest<string, bool>( API_ROUTE_REMOVE_PROMO, code );

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
}