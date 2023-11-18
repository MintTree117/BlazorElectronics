using System.Net.Http.Json;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Outbound.Features;

namespace BlazorElectronics.Client.Services.Features;

public class FeaturesServiceClient : ClientService<FeaturesServiceClient>, IFeaturesServiceClient
{
    readonly HttpClient _http;

    public FeaturesServiceClient( ILogger<FeaturesServiceClient> logger, HttpClient http )
        : base( logger )
    {
        _http = http;
    }
    
    public async Task<ApiReply<FeaturedProductsResponse?>?> GetFeaturedProducts()
    {
        try
        {
            var response = await _http.GetFromJsonAsync<ApiReply<FeaturedProductsResponse?>>( "api/Features/products" );

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
            var response = await _http.GetFromJsonAsync<ApiReply<FeaturedDealsResponse?>>( "api/Features/deals" );

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