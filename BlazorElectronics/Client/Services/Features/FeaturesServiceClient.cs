using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Client.Services.Features;

public class FeaturesServiceClient : CachedClientService<List<FeatureDto>>, IFeaturesServiceClient
{
    const string API_PATH = "api/features/get";

    public FeaturesServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage, 1, "Features" ) { }
    
    public async Task<ServiceReply<List<FeatureDto>?>> GetFeatures()
    {
        List<FeatureDto>? cached = await TryGetCachedItem();

        if ( cached is not null )
            return new ServiceReply<List<FeatureDto>?>( cached );

        ServiceReply<List<FeatureDto>?> reply = await TryGetRequest<List<FeatureDto>?>( API_PATH );

        if ( !reply.Success || reply.Data is null )
            return new ServiceReply<List<FeatureDto>?>( reply.ErrorType, reply.Message );

        await TrySetCachedItem( reply.Data );
        return reply;
    }
}