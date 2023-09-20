using System.Net.Http.Json;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Client.Services.Products;

public class ProductServiceClient : IProductServiceClient
{
    readonly HttpClient _http;
    public List<Product_DTO>? Products { get; set; }

    public ProductServiceClient( HttpClient http )
    {
        _http = http;
    }
    
    public async Task GetProducts()
    {
        var result = await _http.GetFromJsonAsync<ControllerResponse<List<Product_DTO>>>( "api/Product" );

        if ( result is { Data: not null } )
            Products = result.Data;
    }
}