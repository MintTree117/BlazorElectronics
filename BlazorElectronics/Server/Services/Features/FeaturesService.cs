using BlazorElectronics.Server.Dtos;
using BlazorElectronics.Server.Repositories;
using BlazorElectronics.Server.Repositories.Features;
using BlazorElectronics.Shared.Enums;
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
            FeaturesResponse? response = await _repository.Get();

            if ( response is null )
                return new ServiceReply<FeaturesResponse?>( ServiceErrorType.NotFound );

            _cachedFeatures = new CachedObject<FeaturesResponse>( response );

            return new ServiceReply<FeaturesResponse?>( response );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<FeaturesResponse?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<List<CrudView>?>> GetFeaturesView()
    {
        try
        {
            IEnumerable<Feature>? models = await _repository.GetFeatures();
            List<CrudView>? dto = MapFeaturesView( models );

            return dto is not null
                ? new ServiceReply<List<CrudView>?>( dto )
                : new ServiceReply<List<CrudView>?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<List<CrudView>?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<List<CrudView>?>> GetDealsView()
    {
        try
        {
            IEnumerable<FeaturedDeal>? models = await _repository.GetDeals();
            List<CrudView>? dto = MapDealsView( models );

            return dto is not null
                ? new ServiceReply<List<CrudView>?>( dto )
                : new ServiceReply<List<CrudView>?>( ServiceErrorType.NotFound );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<List<CrudView>?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<FeatureEdit?>> GetFeatureEdit( int featureId )
    {
        try
        {
            FeatureEdit? dto = await _repository.GetFeature( featureId );

            return dto is not null
                ? new ServiceReply<FeatureEdit?>( dto )
                : new ServiceReply<FeatureEdit?>( ServiceErrorType.NotFound, NO_DATA_FOUND_MESSAGE );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<FeatureEdit?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<FeaturedDealEdit?>> GetDealEdit( int productId )
    {
        try
        {
            FeaturedDealEdit? dto = await _repository.GetDeal( productId );

            return dto is not null
                ? new ServiceReply<FeaturedDealEdit?>( dto )
                : new ServiceReply<FeaturedDealEdit?>( ServiceErrorType.NotFound, NO_DATA_FOUND_MESSAGE );
        }
        catch ( RepositoryException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<FeaturedDealEdit?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<int>> AddFeature( FeatureEdit dto )
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
    public async Task<ServiceReply<int>> AddDeal( FeaturedDealEdit dto )
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
    public async Task<ServiceReply<bool>> UpdateFeature( FeatureEdit dto )
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
    public async Task<ServiceReply<bool>> UpdateDeal( FeaturedDealEdit dto )
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

    static List<CrudView>? MapFeaturesView( IEnumerable<Feature>? models )
    {
        return models?
            .Select( m => new CrudView { Id = m.FeatureId, Name = m.Name } )
            .ToList();
    }
    static List<CrudView>? MapDealsView( IEnumerable<FeaturedDeal>? models )
    {
        return models?
            .Select( m => new CrudView { Id = m.ProductId, Name = m.ProductName } )
            .ToList();
    }
}