using BlazorElectronics.Server.Dtos;
using BlazorElectronics.Server.Repositories.Features;
using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Server.Services.Features;

public class FeaturesService : ApiService, IFeaturesService
{
    const int CACHE_LIFE = 1;
    readonly IFeaturesRepository _repository;
    CachedObject<FeaturesResponse>? _cachedFeatures;

    public FeaturesService( ILogger<ApiService> logger, IFeaturesRepository repository )
        : base( logger )
    {
        _repository = repository;
    }
    
    public async Task<ServiceReply<FeaturesResponse?>> GetFeatures()
    {
        if ( _cachedFeatures is not null && _cachedFeatures.IsValid( CACHE_LIFE ) )
            return new ServiceReply<FeaturesResponse?>( _cachedFeatures.Object );
        
        try
        {
            FeaturesResponse? result = await _repository.GetView();

            if ( result is null )
                return new ServiceReply<FeaturesResponse?>( ServiceErrorType.NotFound );

            _cachedFeatures = new CachedObject<FeaturesResponse>( result );

            return new ServiceReply<FeaturesResponse?>( result );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<FeaturesResponse?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<FeaturesResponse?>> GetView()
    {
        try
        {
            FeaturesResponse? result = await _repository.GetView();

            return result is not null
                ? new ServiceReply<FeaturesResponse?>( result )
                : new ServiceReply<FeaturesResponse?>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<FeaturesResponse?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> AddFeaturedProduct( FeaturedProductDto dto )
    {
        try
        {
            bool result = await _repository.InsertFeaturedProduct( dto );

            return result
                ? new ServiceReply<bool>( result )
                : new ServiceReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> AddFeaturedDeal( int productId )
    {
        try
        {
            bool result = await _repository.InsertFeaturedDeal( productId );

            return result
                ? new ServiceReply<bool>( result )
                : new ServiceReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<FeaturedProductDto?>> GetFeaturedProductEdit( int productId )
    {
        try
        {
            FeaturedProductDto? result = await _repository.GetFeaturedProductEdit( productId );

            return result is not null
                ? new ServiceReply<FeaturedProductDto?>( result )
                : new ServiceReply<FeaturedProductDto?>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<FeaturedProductDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> UpdateFeaturedProduct( FeaturedProductDto dto )
    {
        try
        {
            bool result = await _repository.UpdateFeaturedProduct( dto );

            return result
                ? new ServiceReply<bool>( result )
                : new ServiceReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> RemoveFeaturedProduct( int productId )
    {
        try
        {
            bool result = await _repository.DeleteFeaturedProduct( productId );

            return result
                ? new ServiceReply<bool>( result )
                : new ServiceReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> RemoveFeaturedDeal( int productId )
    {
        try
        {
            bool result = await _repository.DeleteFeaturedDeal( productId );

            return result
                ? new ServiceReply<bool>( result )
                : new ServiceReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
}