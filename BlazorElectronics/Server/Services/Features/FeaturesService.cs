using BlazorElectronics.Server.Dtos;
using BlazorElectronics.Server.Repositories.Features;
using BlazorElectronics.Shared.Admin.Features;

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
    
    public async Task<ApiReply<FeaturesResponse?>> GetFeatures()
    {
        if ( _cachedFeatures is not null && _cachedFeatures.IsValid( CACHE_LIFE ) )
            return new ApiReply<FeaturesResponse?>( _cachedFeatures.Object );
        
        try
        {
            FeaturesResponse? result = await _repository.GetView();

            if ( result is null )
                return new ApiReply<FeaturesResponse?>( ServiceErrorType.NotFound );

            _cachedFeatures = new CachedObject<FeaturesResponse>( result );

            return new ApiReply<FeaturesResponse?>( result );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<FeaturesResponse?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ApiReply<FeaturesResponse?>> GetView()
    {
        try
        {
            FeaturesResponse? result = await _repository.GetView();

            return result is not null
                ? new ApiReply<FeaturesResponse?>( result )
                : new ApiReply<FeaturesResponse?>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<FeaturesResponse?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ApiReply<bool>> AddFeaturedProduct( FeaturedProductEditDto dto )
    {
        try
        {
            bool result = await _repository.InsertFeaturedProduct( dto );

            return result
                ? new ApiReply<bool>( result )
                : new ApiReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ApiReply<bool>> AddFeaturedDeal( int productId )
    {
        try
        {
            bool result = await _repository.InsertFeaturedDeal( productId );

            return result
                ? new ApiReply<bool>( result )
                : new ApiReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ApiReply<FeaturedProductEditDto?>> GetFeaturedProductEdit( int productId )
    {
        try
        {
            FeaturedProductEditDto? result = await _repository.GetFeaturedProductEdit( productId );

            return result is not null
                ? new ApiReply<FeaturedProductEditDto?>( result )
                : new ApiReply<FeaturedProductEditDto?>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<FeaturedProductEditDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ApiReply<bool>> UpdateFeaturedProduct( FeaturedProductEditDto dto )
    {
        try
        {
            bool result = await _repository.UpdateFeaturedProduct( dto );

            return result
                ? new ApiReply<bool>( result )
                : new ApiReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ApiReply<bool>> RemoveFeaturedProduct( int productId )
    {
        try
        {
            bool result = await _repository.DeleteFeaturedProduct( productId );

            return result
                ? new ApiReply<bool>( result )
                : new ApiReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ApiReply<bool>> RemoveFeaturedDeal( int productId )
    {
        try
        {
            bool result = await _repository.DeleteFeaturedDeal( productId );

            return result
                ? new ApiReply<bool>( result )
                : new ApiReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ApiReply<bool>( ServiceErrorType.ServerError );
        }
    }
}