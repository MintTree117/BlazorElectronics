using System.Net.Http.Json;
using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Outbound.Features;

namespace BlazorElectronics.Client.Services.Features;

public class FeaturesServiceClient : ClientService, IFeaturesServiceClient
{
    public FeaturesServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }

    public async Task<ApiReply<FeaturedProductsResponse?>?> GetFeaturedProducts()
    {
        try
        {
            var response = await Http.GetFromJsonAsync<ApiReply<FeaturedProductsResponse?>>( "api/Features/products" );

            if ( response == null )
                return new ApiReply<FeaturedProductsResponse?>( null, false, "Service response is null!" );
            
            return !response.Success 
                ? new ApiReply<FeaturedProductsResponse?>( null, false, response.Message ??= "Failed to retrieve Featured Products; message is null!" ) 
                : response;
        }
        catch ( Exception e )
        {
            return new ApiReply<FeaturedProductsResponse?>( null, false, e.Message );
        }
    }
    public async Task<ApiReply<FeaturedDealsResponse?>?> GetFeaturedDeals()
    {
        try
        {
            var response = await Http.GetFromJsonAsync<ApiReply<FeaturedDealsResponse?>>( "api/Features/deals" );

            if ( response == null )
                return new ApiReply<FeaturedDealsResponse?>( null, false, "Service response is null!" );
            
            return !response.Success 
                ? new ApiReply<FeaturedDealsResponse?>( null, false, response.Message ??= "Failed to retrieve Featured Deals; message is null!" ) 
                : response;
        }
        catch ( Exception e )
        {
            return new ApiReply<FeaturedDealsResponse?>( null, false, e.Message );
        }
    }
}