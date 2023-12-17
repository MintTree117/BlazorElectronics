using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models;
using BlazorElectronics.Server.Data;
using BlazorElectronics.Server.Services;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Server.Core.Services;

public class FeaturesService : ApiService, IFeaturesService
{
    const int CACHE_LIFE = 1;
    readonly IFeaturesRepository _repository;
    CachedObject<FeaturesReplyDto>? _cachedFeatures;

    public FeaturesService( ILogger<ApiService> logger, IFeaturesRepository repository )
        : base( logger )
    {
        _repository = repository;
    }
    
    public async Task<ServiceReply<FeaturesReplyDto?>> GetFeatures()
    {
        if ( _cachedFeatures is not null && _cachedFeatures.IsValid( CACHE_LIFE ) )
            return new ServiceReply<FeaturesReplyDto?>( _cachedFeatures.Object );
        
        try
        {
            FeaturesReplyDto? response = await _repository.Get();

            if ( response is null )
                return new ServiceReply<FeaturesReplyDto?>( ServiceErrorType.NotFound );

            _cachedFeatures = new CachedObject<FeaturesReplyDto>( response );

            return new ServiceReply<FeaturesReplyDto?>( response );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<FeaturesReplyDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<List<CrudViewDto>?>> GetFeaturesView()
    {
        try
        {
            IEnumerable<FeatureDto>? models = await _repository.GetFeatures();
            List<CrudViewDto>? dto = MapFeaturesView( models );

            return dto is not null
                ? new ServiceReply<List<CrudViewDto>?>( dto )
                : new ServiceReply<List<CrudViewDto>?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<List<CrudViewDto>?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<List<CrudViewDto>?>> GetDealsView()
    {
        try
        {
            IEnumerable<FeaturedDealDto>? models = await _repository.GetDeals();
            List<CrudViewDto>? dto = MapDealsView( models );

            return dto is not null
                ? new ServiceReply<List<CrudViewDto>?>( dto )
                : new ServiceReply<List<CrudViewDto>?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<List<CrudViewDto>?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<FeatureDtoEditDto?>> GetFeatureEdit( int featureId )
    {
        try
        {
            FeatureDtoEditDto? dto = await _repository.GetFeature( featureId );

            return dto is not null
                ? new ServiceReply<FeatureDtoEditDto?>( dto )
                : new ServiceReply<FeatureDtoEditDto?>( ServiceErrorType.NotFound, NO_DATA_FOUND_MESSAGE );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<FeatureDtoEditDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<FeaturedDealDtoEditDto?>> GetDealEdit( int productId )
    {
        try
        {
            FeaturedDealDtoEditDto? dto = await _repository.GetDeal( productId );

            return dto is not null
                ? new ServiceReply<FeaturedDealDtoEditDto?>( dto )
                : new ServiceReply<FeaturedDealDtoEditDto?>( ServiceErrorType.NotFound, NO_DATA_FOUND_MESSAGE );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<FeaturedDealDtoEditDto?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<int>> AddFeature( FeatureDtoEditDto dto )
    {
        try
        {
            int id = await _repository.InsertFeature( dto );

            return id > 0
                ? new ServiceReply<int>( id )
                : new ServiceReply<int>( ServiceErrorType.NotFound, NO_DATA_FOUND_MESSAGE );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<int>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<int>> AddDeal( FeaturedDealDtoEditDto dto )
    {
        try
        {
            int id = await _repository.InsertDeal( dto );

            return id > 0
                ? new ServiceReply<int>( id )
                : new ServiceReply<int>( ServiceErrorType.NotFound, NO_DATA_FOUND_MESSAGE );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<int>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> UpdateFeature( FeatureDtoEditDto dto )
    {
        try
        {
            bool result = await _repository.UpdateFeature( dto );

            return result
                ? new ServiceReply<bool>( result )
                : new ServiceReply<bool>( ServiceErrorType.NotFound, NO_DATA_FOUND_MESSAGE );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> UpdateDeal( FeaturedDealDtoEditDto dto )
    {
        try
        {
            bool result = await _repository.UpdateDeal( dto );

            return result
                ? new ServiceReply<bool>( result )
                : new ServiceReply<bool>( ServiceErrorType.NotFound, NO_DATA_FOUND_MESSAGE );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> RemoveFeature( int featureId )
    {
        try
        {
            bool result = await _repository.DeleteFeature( featureId );

            return result
                ? new ServiceReply<bool>( result )
                : new ServiceReply<bool>( ServiceErrorType.NotFound, NO_DATA_FOUND_MESSAGE );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> RemoveDeal( int productId )
    {
        try
        {
            bool result = await _repository.DeleteDeal( productId );

            return result
                ? new ServiceReply<bool>( result )
                : new ServiceReply<bool>( ServiceErrorType.NotFound, NO_DATA_FOUND_MESSAGE );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }

    static List<CrudViewDto>? MapFeaturesView( IEnumerable<FeatureDto>? models )
    {
        return models?
            .Select( m => new CrudViewDto { Id = m.FeatureId, Name = m.Name } )
            .ToList();
    }
    static List<CrudViewDto>? MapDealsView( IEnumerable<FeaturedDealDto>? models )
    {
        return models?
            .Select( m => new CrudViewDto { Id = m.ProductId, Name = m.ProductName } )
            .ToList();
    }
}