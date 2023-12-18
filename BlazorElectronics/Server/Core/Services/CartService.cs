using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Services;
using BlazorElectronics.Shared.Cart;
using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Server.Core.Services;

public class CartService : ApiService, ICartService
{
    readonly ICartRepository _cartRepository;

    public CartService( ILogger<ApiService> logger, ICartRepository cartRepository )
        : base( logger )
    {
        _cartRepository = cartRepository;
    }

    public async Task<ServiceReply<CartReplyDto?>> GetCart( int userId )
    {
        try
        {
            IEnumerable<CartProductDto>? models = await _cartRepository.GetCart( userId );
            CartReplyDto? response = MapCartResponse( models );

            return response is not null
                ? new ServiceReply<CartReplyDto?>( response )
                : new ServiceReply<CartReplyDto?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<CartReplyDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<CartReplyDto?>> UpdateCart( int userId, CartRequestDto requestDto )
    {
        try
        {
            IEnumerable<CartProductDto>? models = await _cartRepository.UpdateCart( userId, requestDto );
            CartReplyDto? response = MapCartResponse( models );

            return response is not null
                ? new ServiceReply<CartReplyDto?>( response )
                : new ServiceReply<CartReplyDto?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<CartReplyDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<CartReplyDto?>> AddToCart( int userId, CartItemDto itemDto )
    {
        try
        {
            IEnumerable<CartProductDto>? models = await _cartRepository.InsertItem( userId, itemDto );
            CartReplyDto? response = MapCartResponse( models );

            return response is not null
                ? new ServiceReply<CartReplyDto?>( response )
                : new ServiceReply<CartReplyDto?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<CartReplyDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<CartReplyDto?>> UpdateQuantity( int userId, CartItemDto itemDto )
    {
        try
        {
            IEnumerable<CartProductDto>? models = await _cartRepository.UpdateQuantity( userId, itemDto );
            CartReplyDto? response = MapCartResponse( models );

            return response is not null
                ? new ServiceReply<CartReplyDto?>( response )
                : new ServiceReply<CartReplyDto?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<CartReplyDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<CartReplyDto?>> RemoveFromCart( int userId, int productId )
    {
        try
        {
            IEnumerable<CartProductDto>? models = await _cartRepository.DeleteFromCart( userId, productId );
            CartReplyDto? response = MapCartResponse( models );

            return response is not null
                ? new ServiceReply<CartReplyDto?>( response )
                : new ServiceReply<CartReplyDto?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<CartReplyDto?>( ServiceErrorType.ServerError );
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
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }

    static CartReplyDto? MapCartResponse( IEnumerable<CartProductDto>? models )
    {
        if ( models is null )
            return null;

        return new CartReplyDto
        {
            Items = models.ToList()
        };
    }
}