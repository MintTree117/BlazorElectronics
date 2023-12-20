using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Shared.Cart;
using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Server.Core.Services;

public sealed class CartService : ApiService, ICartService
{
    readonly ICartRepository _cartRepository;

    public CartService( ILogger<ApiService> logger, ICartRepository cartRepository )
        : base( logger )
    {
        _cartRepository = cartRepository;
    }
    
    public async Task<ServiceReply<List<CartProductDto>?>> UpdateCart( int userId, List<CartItemDto> items )
    {
        try
        {
            IEnumerable<CartProductDto>? models = await _cartRepository.UpdateCart( userId, items );
            List<CartProductDto>? reply = models?.ToList();

            return reply is not null
                ? new ServiceReply<List<CartProductDto>?>( reply )
                : new ServiceReply<List<CartProductDto>?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<List<CartProductDto>?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> AddToCart( int userId, CartItemDto itemDto )
    {
        try
        {
            bool success = await _cartRepository.InsertOrUpdateItem( userId, itemDto );

            return success
                ? new ServiceReply<bool>( true )
                : new ServiceReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> RemoveFromCart( int userId, int productId )
    {
        try
        {
            bool success = await _cartRepository.DeleteItem( userId, productId );

            return success
                ? new ServiceReply<bool>( true )
                : new ServiceReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
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
}