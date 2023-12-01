using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Client.Services.Features;

public class FeaturesServiceClient : ClientService, IFeaturesServiceClient
{
    const string API_PATH = "api/features/get";
    FeaturesResponse? _features;
    
    public FeaturesServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public async Task<ServiceReply<List<FeaturedProductDto>?>> GetFeaturedProducts()
    {
        ServiceReply<bool> reply = await GetFeatures();

        return reply.Success && _features is not null
            ? new ServiceReply<List<FeaturedProductDto>?>( _features.FeaturedProducts )
            : new ServiceReply<List<FeaturedProductDto>?>( reply.ErrorType, reply.Message );
    }
    public async Task<ServiceReply<List<FeaturedDealDto>?>> GetFeaturedDeals()
    {
        ServiceReply<bool> reply = await GetFeatures();

        return reply.Success && _features is not null
            ? new ServiceReply<List<FeaturedDealDto>?>( _features.FeaturedDeals )
            : new ServiceReply<List<FeaturedDealDto>?>( reply.ErrorType, reply.Message );
    }

    async Task<ServiceReply<bool>> GetFeatures()
    {
        if ( _features is not null )
            return new ServiceReply<bool>( true );

        ServiceReply<FeaturesResponse?> reply = await TryGetRequest<FeaturesResponse?>( API_PATH );
        _features = reply.Data;

        return _features is not null
            ? new ServiceReply<bool>( true )
            : new ServiceReply<bool>( reply.ErrorType, reply.Message );
    }
}