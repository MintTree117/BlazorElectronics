using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Client.Services.Features;

public class FeaturedDealsServiceClient : CachedClientService<List<FeatureDealDto>>, IFeaturedDealsServiceClient
{
    const string API_ROUTE = "api/features";
    const string API_ROUTE_FRONT_PAGE = $"{API_ROUTE}/get-featured-deals-home";
    const string API_ROUTE_GET_DEALS = $"{API_ROUTE}/get-featured-deals";
    
    public FeaturedDealsServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage, 1, "Front Page Deals" ) { }

    public async Task<ServiceReply<List<FeatureDealDto>?>> GetFrontPageDeals()
    {
        List<FeatureDealDto>? cached = await TryGetCachedItem();

        if ( cached is not null )
            return new ServiceReply<List<FeatureDealDto>?>( cached );

        ServiceReply<List<FeatureDealDto>?> reply = await TryGetRequest<List<FeatureDealDto>?>( API_ROUTE_FRONT_PAGE );

        if ( !reply.Success || reply.Payload is null )
            return new ServiceReply<List<FeatureDealDto>?>( reply.ErrorType, reply.Message );

        await TrySetCachedItem( reply.Payload );
        return reply;
    }
    public async Task<ServiceReply<List<FeatureDealDto>?>> GetFeatureDeals( PaginationDto pagination )
    {
        return await TryPostRequest<List<FeatureDealDto>?>( API_ROUTE_GET_DEALS, pagination );
    }
}