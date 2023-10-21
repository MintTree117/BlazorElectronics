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
    
    public async Task<ServiceResponse<FeaturedProducts_DTO?>?> GetFeaturedProducts()
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<FeaturedProducts_DTO?>>( "api/Features/products" );
        return result;
    }
    public async Task<ServiceResponse<FeaturesDeals_DTO?>?> GetFeaturedDeals()
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<FeaturesDeals_DTO?>>( "api/Features/deals" );
        return result;
    }
}