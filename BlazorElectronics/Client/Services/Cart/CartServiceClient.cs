using Blazored.LocalStorage;
using BlazorElectronics.Client.Services.Users;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Cart;

namespace BlazorElectronics.Client.Services.Cart;

public class CartServiceClient : UserServiceClient, ICartServiceClient
{
    public event Action<int>? OnChange;
    const string CART_STORAGE_KEY = "cart";

    const string API_ROUTE = "api/cart";
    const string API_ROUTE_GET = $"{API_ROUTE}/get";
    const string API_ROUTE_UPDATE = $"{API_ROUTE}/update";
    const string API_ROUTE_ADD = $"{API_ROUTE}/add";
    const string API_ROUTE_QUANTITY = $"{API_ROUTE}/quantity";
    const string API_ROUTE_REMOVE = $"{API_ROUTE}/remove";
    const string API_ROUTE_CLEAR = $"{API_ROUTE}/clear";
    
    public CartServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public async Task<ServiceReply<CartReplyDto?>> GetCart()
    {
        ServiceReply<CartReplyDto?> reply = await TryUserRequest<CartReplyDto>( API_ROUTE_GET );
        await HandleCartReply( reply );
        return reply;
    }
    public async Task<ServiceReply<bool>> UpdateCart()
    {
        CartRequestDto request = await GetCartRequest();
        ServiceReply<CartReplyDto?> reply = await TryUserRequest<CartRequestDto, CartReplyDto>( API_ROUTE_UPDATE, request );
        return await HandleCartReply( reply );
    }
    public async Task<ServiceReply<bool>> AddToCart( CartItemDto itemDto )
    {
        ServiceReply<CartReplyDto?> reply = await TryUserRequest<CartItemDto, CartReplyDto>( API_ROUTE_ADD, itemDto );
        return await HandleCartReply( reply );
    }
    public async Task<ServiceReply<bool>> UpdateCartQuantity( CartItemDto itemDto )
    {
        ServiceReply<CartReplyDto?> reply = await TryUserRequest<CartItemDto, CartReplyDto>( API_ROUTE_QUANTITY, itemDto );
        return await HandleCartReply( reply );
    }
    public async Task<ServiceReply<bool>> RemoveFromCart( int productId )
    {
        IntDto intDto = new( productId );
        ServiceReply<CartReplyDto?> reply = await TryUserRequest<IntDto, CartReplyDto>( API_ROUTE_REMOVE, intDto );
        return await HandleCartReply( reply );
    }
    public async Task<ServiceReply<bool>> ClearCart()
    {
        await Storage.SetItemAsync( CART_STORAGE_KEY, new CartReplyDto() );
        return await TryUserRequest<bool>( API_ROUTE_CLEAR );
    }

    async Task<ServiceReply<bool>> HandleCartReply( ServiceReply<CartReplyDto?> reply )
    {
        if ( !reply.Success || reply.Data is null )
            return new ServiceReply<bool>( reply.ErrorType, reply.Message );

        await Storage.SetItemAsync( CART_STORAGE_KEY, reply.Data );
        return new ServiceReply<bool>( true );
    }
    async Task<CartRequestDto> GetCartRequest()
    {
        CartReplyDto cart = await GetLocalCart();
        return new CartRequestDto( cart );
    }
    async Task<CartReplyDto> GetLocalCart()
    {
        try
        {
            var cart = await Storage.GetItemAsync<CartReplyDto>( CART_STORAGE_KEY );
            return cart ?? new CartReplyDto();
        }
        catch ( Exception e )
        {
            Logger.LogError( e.Message, e );
            return new CartReplyDto();
        }
    }
}