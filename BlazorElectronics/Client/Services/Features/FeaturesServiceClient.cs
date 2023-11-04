using System.Net.Http.Json;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Outbound.Features;

namespace BlazorElectronics.Client.Services.Features;

public class FeaturesServiceClient : IFeaturesServiceClient
{
    readonly HttpClient _http;

    public FeaturesServiceClient( HttpClient http )
    {
        _http = http;
    }
    
    public async Task<Reply<FeaturedProducts_DTO?>?> GetFeaturedProducts()
    {
        try
        {
            var response = await _http.GetFromJsonAsync<Reply<FeaturedProducts_DTO?>>( "api/Features/products" );

            if ( response == null )
                return new Reply<FeaturedProducts_DTO?>( null, false, "Service response is null!" );
            
            return !response.Success 
                ? new Reply<FeaturedProducts_DTO?>( null, false, response.Message ??= "Failed to retrieve Featured Products; message is null!" ) 
                : response;
        }
        catch ( Exception e )
        {
            return new Reply<FeaturedProducts_DTO?>( null, false, e.Message );
        }
    }
    public async Task<Reply<FeaturedDeals_DTO?>?> GetFeaturedDeals()
    {
        try
        {
            var response = await _http.GetFromJsonAsync<Reply<FeaturedDeals_DTO?>>( "api/Features/deals" );

            if ( response == null )
                return new Reply<FeaturedDeals_DTO?>( null, false, "Service response is null!" );
            
            return !response.Success 
                ? new Reply<FeaturedDeals_DTO?>( null, false, response.Message ??= "Failed to retrieve Featured Deals; message is null!" ) 
                : response;
        }
        catch ( Exception e )
        {
            return new Reply<FeaturedDeals_DTO?>( null, false, e.Message );
        }
    }
}