using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Client.Services.Features;

public class FeaturesServiceClient : ClientService, IFeaturesServiceClient
{
    const string API_PATH = "api/features";
    const string API_PATH_FEATURES = $"{API_PATH}/get-features";
    const string API_PATH_DEALS = $"{API_PATH}/get-deals";
    
    List<FeatureDto>? _cachedFeatures;
    List<FeatureDealDto>? _cachedDealsFrontPage;
    
    public FeaturesServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public async Task<ServiceReply<List<FeatureDto>?>> GetFeatures()
    {
        return await GetFeaturesReply();
    }
    public async Task<ServiceReply<List<FeatureDealDto>?>> GetFeatureDeals( PaginationDto pagination )
    {
        return await GetDealsReply( pagination );
    }

    async Task<ServiceReply<List<FeatureDto>?>> GetFeaturesReply()
    {
        if ( _cachedFeatures is not null )
            return new ServiceReply<List<FeatureDto>?>( _cachedFeatures );

        ServiceReply<List<FeatureDto>?> reply = await TryGetRequest<List<FeatureDto>?>( API_PATH_FEATURES );
        _cachedFeatures = reply.Data;

        return reply;
    }
    async Task<ServiceReply<List<FeatureDealDto>?>> GetDealsReply( PaginationDto pagination )
    {
        if ( pagination.Page == 1 && _cachedDealsFrontPage is not null )
            return new ServiceReply<List<FeatureDealDto>?>( _cachedDealsFrontPage );

        ServiceReply<List<FeatureDealDto>?> reply = await TryPostRequest<List<FeatureDealDto>?>( API_PATH_DEALS, pagination );
        
        if (pagination.Page == 1)
            _cachedDealsFrontPage = reply.Data;

        return _cachedFeatures is not null
            ? new ServiceReply<List<FeatureDealDto>?>( reply.Data )
            : new ServiceReply<List<FeatureDealDto>?>( reply.ErrorType, reply.Message );
    }
}