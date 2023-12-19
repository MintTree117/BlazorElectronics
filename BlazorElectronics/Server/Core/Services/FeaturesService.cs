using BlazorElectronics.Server.Api.Interfaces;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models;
using BlazorElectronics.Shared.Enums;
using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Server.Core.Services;

public sealed class FeaturesService : ApiService, IFeaturesService
{
    const int CACHE_LIFE = 1;
    readonly IFeaturesRepository _repository;
    CachedObject<List<FeatureDto>>? _cachedFeatures;
    CachedObject<List<FeatureDealDto>>? _cachedDealsFrontPage;

    public FeaturesService( ILogger<ApiService> logger, IFeaturesRepository repository )
        : base( logger )
    {
        _repository = repository;
    }

    public async Task<ServiceReply<List<FeatureDealDto>?>> GetDeals( int rows, int page )
    {
        if ( page == 1 && CacheValid( _cachedDealsFrontPage ) )
            return new ServiceReply<List<FeatureDealDto>?>( _cachedDealsFrontPage!.Object );

        try
        {
            int offset = rows * Math.Max( page - 1, 1 );
            IEnumerable<FeatureDealDto>? models = await _repository.GetDeals( rows, offset );

            if ( models is null )
                return new ServiceReply<List<FeatureDealDto>?>( ServiceErrorType.NotFound );

            List<FeatureDealDto> dto = models.ToList();

            if ( page == 1 )
                _cachedDealsFrontPage = new CachedObject<List<FeatureDealDto>>( dto );

            return new ServiceReply<List<FeatureDealDto>?>( dto );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<List<FeatureDealDto>?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<List<FeatureDto>?>> GetFeatures()
    {
        if ( CacheValid( _cachedFeatures ) )
            return new ServiceReply<List<FeatureDto>?>( _cachedFeatures!.Object );
        
        try
        {
            IEnumerable<FeatureDto>? models = await _repository.GetFeatures();

            if ( models is null )
                return new ServiceReply<List<FeatureDto>?>( ServiceErrorType.NotFound );

            List<FeatureDto> dto = models.ToList();
            _cachedFeatures = new CachedObject<List<FeatureDto>>( dto );

            return new ServiceReply<List<FeatureDto>?>( dto );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<List<FeatureDto>?>( ServiceErrorType.ServerError );
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
    public async Task<ServiceReply<FeatureDtoEditDto?>> GetFeatureEdit( int featureId )
    {
        try
        {
            FeatureDtoEditDto? dto = await _repository.GetFeatureEdit( featureId );

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

    static List<CrudViewDto>? MapFeaturesView( IEnumerable<FeatureDto>? models )
    {
        return models?
            .Select( m => new CrudViewDto { Id = m.FeatureId, Name = m.Name } )
            .ToList();
    }
    bool CacheValid<T>( CachedObject<T>? _cache )
    {
        return _cache is not null && _cache.IsValid( CACHE_LIFE );
    }
}