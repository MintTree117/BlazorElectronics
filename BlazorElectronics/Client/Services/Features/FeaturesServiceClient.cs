using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Client.Services.Features;

public class FeaturesServiceClient : ClientService, IFeaturesServiceClient
{
    const string API_PATH = "api/features/get";
    FeaturesResponse? _response;
    
    public FeaturesServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public async Task<ServiceReply<List<Feature>?>> GetFeatures()
    {
        ServiceReply<bool> reply = await GetResponse();

        return reply.Success && _response is not null
            ? new ServiceReply<List<Feature>?>( _response.Features )
            : new ServiceReply<List<Feature>?>( reply.ErrorType, reply.Message );
    }
    public async Task<ServiceReply<List<FeaturedDeal>?>> GetFeaturedDeals()
    {
        ServiceReply<bool> reply = await GetResponse();

        return reply.Success && _response is not null
            ? new ServiceReply<List<FeaturedDeal>?>( _response.Deals )
            : new ServiceReply<List<FeaturedDeal>?>( reply.ErrorType, reply.Message );
    }

    async Task<ServiceReply<bool>> GetResponse()
    {
        if ( _response is not null )
            return new ServiceReply<bool>( true );

        ServiceReply<FeaturesResponse?> reply = await TryGetRequest<FeaturesResponse?>( API_PATH );
        _response = reply.Data;

        return _response is not null
            ? new ServiceReply<bool>( true )
            : new ServiceReply<bool>( reply.ErrorType, reply.Message );
    }
}