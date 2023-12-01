using BlazorElectronics.Server.Repositories.Cart;
using BlazorElectronics.Shared.Cart;

namespace BlazorElectronics.Server.Services.Cart;

public class CartService : ApiService, ICartService
{
    readonly ICartRepository _cartRepository;

    public CartService( ILogger<ApiService> logger, ICartRepository cartRepository )
        : base( logger )
    {
        _cartRepository = cartRepository;
    }

    public async Task<ServiceReply<CartResponse?>> UpdateCart( int userId, CartRequest request )
    {
        try
        {
            IEnumerable<CartProductResponse>? models = await _cartRepository.UpdateCart( userId, request );
            CartResponse? response = MapCartResponse( models );

            return response is not null
                ? new ServiceReply<CartResponse?>( response )
                : new ServiceReply<CartResponse?>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<CartResponse?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<CartResponse?>> GetCart( int userId )
    {
        try
        {
            IEnumerable<CartProductResponse>? models = await _cartRepository.GetCart( userId );
            CartResponse? response = MapCartResponse( models );

            return response is not null
                ? new ServiceReply<CartResponse?>( response )
                : new ServiceReply<CartResponse?>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<CartResponse?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<CartResponse?>> AddToCart( int userId, CartItemDto item )
    {
        try
        {
            IEnumerable<CartProductResponse>? models = await _cartRepository.InsertItem( userId, item );
            CartResponse? response = MapCartResponse( models );

            return response is not null
                ? new ServiceReply<CartResponse?>( response )
                : new ServiceReply<CartResponse?>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<CartResponse?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<CartResponse?>> UpdateQuantity( int userId, CartItemDto item )
    {
        try
        {
            IEnumerable<CartProductResponse>? models = await _cartRepository.UpdateQuantity( userId, item );
            CartResponse? response = MapCartResponse( models );

            return response is not null
                ? new ServiceReply<CartResponse?>( response )
                : new ServiceReply<CartResponse?>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<CartResponse?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<CartResponse?>> RemoveFromCart( int userId, int productId )
    {
        try
        {
            IEnumerable<CartProductResponse>? models = await _cartRepository.DeleteFromCart( userId, productId );
            CartResponse? response = MapCartResponse( models );

            return response is not null
                ? new ServiceReply<CartResponse?>( response )
                : new ServiceReply<CartResponse?>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<CartResponse?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> ClearCart( int userId )
    {
        try
        {
            bool result = await _cartRepository.DeleteCart( userId );
            
            return result
                ? new ServiceReply<bool>( true )
                : new ServiceReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }

    static CartResponse? MapCartResponse( IEnumerable<CartProductResponse>? models )
    {
        if ( models is null )
            return null;

        return new CartResponse
        {
            Items = models.ToList()
        };
    }
}