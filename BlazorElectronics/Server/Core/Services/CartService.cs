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
    
    public async Task<ServiceReply<CartDto?>> UpdateCart( int userId, List<CartItemDto> items )
    {
        try
        {
            CartDto? model = await _cartRepository.UpdateCart( userId, items );

            return model is not null
                ? new ServiceReply<CartDto?>( model )
                : new ServiceReply<CartDto?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<CartDto?>( ServiceErrorType.ServerError );
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