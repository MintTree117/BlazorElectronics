using BlazorElectronics.Server.Dtos;
using BlazorElectronics.Server.Models.Features;
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
            FeaturesModel? model = await _repository.Get();

            if ( model is null )
                return new ServiceReply<FeaturesResponse?>( ServiceErrorType.NotFound );

            FeaturesResponse r = new()
            {
                Features = model.Features is not null ? model.Features.ToList() : new List<Feature>(),
                Deals = model.Deals is not null ? model.Deals.ToList() : new List<FeaturedDeal>()
            };

            _cachedFeatures = new CachedObject<FeaturesResponse>( r );

            return new ServiceReply<FeaturesResponse?>( r );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<FeaturesResponse?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<List<AdminItemViewDto>?>> GetFeaturesView()
    {
        try
        {
            IEnumerable<Feature>? models = await _repository.GetFeatures();
            List<AdminItemViewDto>? dto = MapFeaturesView( models );

            return dto is not null
                ? new ServiceReply<List<AdminItemViewDto>?>( dto )
                : new ServiceReply<List<AdminItemViewDto>?>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<List<AdminItemViewDto>?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<List<AdminItemViewDto>?>> GetDealsView()
    {
        try
        {
            IEnumerable<FeaturedDeal>? models = await _repository.GetDeals();
            List<AdminItemViewDto>? dto = MapDealsView( models );

            return dto is not null
                ? new ServiceReply<List<AdminItemViewDto>?>( dto )
                : new ServiceReply<List<AdminItemViewDto>?>( ServiceErrorType.NotFound );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<List<AdminItemViewDto>?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<Feature?>> GetFeature( int featureId )
    {
        try
        {
            Feature? dto = await _repository.GetFeature( featureId );

            return dto is not null
                ? new ServiceReply<Feature?>( dto )
                : new ServiceReply<Feature?>( ServiceErrorType.NotFound, NO_DATA_FOUND_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<Feature?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<FeaturedDeal?>> GetDeal( int productId )
    {
        try
        {
            FeaturedDeal? dto = await _repository.GetDeal( productId );

            return dto is not null
                ? new ServiceReply<FeaturedDeal?>( dto )
                : new ServiceReply<FeaturedDeal?>( ServiceErrorType.NotFound, NO_DATA_FOUND_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<FeaturedDeal?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<Feature?>> AddFeature( Feature dto )
    {
        try
        {
            Feature? result = await _repository.InsertFeature( dto );

            return result is not null
                ? new ServiceReply<Feature?>( result )
                : new ServiceReply<Feature?>( ServiceErrorType.NotFound, NO_DATA_FOUND_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<Feature?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<FeaturedDeal?>> AddDeal( FeaturedDeal dto )
    {
        try
        {
            FeaturedDeal? result = await _repository.InsertDeal( dto );

            return result is not null
                ? new ServiceReply<FeaturedDeal?>( result )
                : new ServiceReply<FeaturedDeal?>( ServiceErrorType.NotFound, NO_DATA_FOUND_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<FeaturedDeal?>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> UpdateFeature( Feature dto )
    {
        try
        {
            bool result = await _repository.UpdateFeature( dto );

            return result
                ? new ServiceReply<bool>( result )
                : new ServiceReply<bool>( ServiceErrorType.NotFound, NO_DATA_FOUND_MESSAGE );
        }
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<ServiceReply<bool>> UpdateDeal( FeaturedDeal dto )
    {
        try
        {
            bool result = await _repository.UpdateDeal( dto );

            return result
                ? new ServiceReply<bool>( result )
                : new ServiceReply<bool>( ServiceErrorType.NotFound, NO_DATA_FOUND_MESSAGE );
        }
        catch ( ServiceException e )
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
        catch ( ServiceException e )
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
        catch ( ServiceException e )
        {
            Logger.LogError( e.Message, e );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }

    static List<AdminItemViewDto>? MapFeaturesView( IEnumerable<Feature>? models )
    {
        return models?
            .Select( m => new AdminItemViewDto { Id = m.FeatureId, Name = m.FeatureName } )
            .ToList();
    }
    static List<AdminItemViewDto>? MapDealsView( IEnumerable<FeaturedDeal>? models )
    {
        return models?
            .Select( m => new AdminItemViewDto { Id = m.ProductId, Name = m.ProductName } )
            .ToList();
    }
}