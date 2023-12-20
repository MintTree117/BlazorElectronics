using Blazored.LocalStorage;
using BlazorElectronics.Client.Models;
using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Cart;
using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Client.Services.Cart;

public class CartServiceClient : UserServiceClient, ICartServiceClient
{
    const string CART_STORAGE_KEY = "cart";
    
    const string API_ROUTE = "api/cart";
    const string API_ROUTE_UPDATE = $"{API_ROUTE}/update";
    const string API_ROUTE_ADD = $"{API_ROUTE}/add-update";
    const string API_ROUTE_REMOVE = $"{API_ROUTE}/remove";
    const string API_ROUTE_CLEAR = $"{API_ROUTE}/clear";
    
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
        ServiceReply<List<CartProductDto>?> reply = await TryUserRequest<List<CartItemDto>, List<CartProductDto>>( API_ROUTE_UPDATE, localItems );

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
        CartProductDto? item = cart.Items.Find( p => p.ProductId == productId );

        if ( item != null )
            return new ServiceReply<int>( item.ItemQuantity );

        ServiceReply<CartModel?> updateReply = await UpdateCart();
        item = updateReply.Data?.Items.Find( p => p.ProductId == productId );

        return item is not null
            ? new ServiceReply<int>( item.ItemQuantity )
            : new ServiceReply<int>( updateReply.ErrorType, updateReply.Message );
    }
    public async Task<ServiceReply<bool>> AddOrUpdateItem( CartProductDto product )
    {
        CartItemDto dto = new( product.ProductId, product.ItemQuantity );
        ServiceReply<bool> reply = await TryUserRequest<CartItemDto, bool>( API_ROUTE_ADD, dto );
        
        CartModel cart = await GetLocalCart();
        cart.Add( product );
        bool storedLocally = await TryStoreLocalCart( cart );

        if ( !reply.Success || !storedLocally )
            return new ServiceReply<bool>( ServiceErrorType.NotFound, "Failed to add item to local storage or server!" );
        
        OnChange?.Invoke( cart.GetCartInfo() );
        return new ServiceReply<bool>( true );
    }
    public async Task<ServiceReply<bool>> RemoveItem( int productId )
    {
        IntDto intDto = new( productId );
        ServiceReply<bool> reply = await TryUserRequest<IntDto, bool>( API_ROUTE_REMOVE, intDto );
        
        CartModel cart = await GetLocalCart();
        cart.Remove( productId );
        bool storedLocally = await TryStoreLocalCart( cart );

        if ( !reply.Success || !storedLocally )
            return new ServiceReply<bool>( ServiceErrorType.NotFound, "Failed to remove item from local storage or server!" );

        OnChange?.Invoke( cart.GetCartInfo() );
        return new ServiceReply<bool>( true );
    }
    public async Task<ServiceReply<bool>> ClearCart()
    {
        await TryStoreLocalCart( new CartModel() );
        return await TryUserRequest<bool>( API_ROUTE_CLEAR );
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