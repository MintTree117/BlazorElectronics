using System.Net.Http.Json;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Client.Services.Products;

public class ProductServiceClient : IProductServiceClient
{
    readonly HttpClient _http;
    public ProductDetails_DTO? ProductDetails { get; set; }
    public List<Product_DTO>? Products { get; set; }
    
    public ProductServiceClient( HttpClient http )
    {
        _http = http;
    }
    
    public async Task GetProducts()
    {
        var result = await _http.GetFromJsonAsync<ControllerResponse<List<Product_DTO>>>( "api/Product/products" );

        if ( result is { Data: not null } )
            Products = result.Data;
    }
    public async Task GetProductDetails( int productId )
    {
        var result = await _http.GetFromJsonAsync<ControllerResponse<ProductDetails_DTO>>( $"api/Product/product_details/{productId}" );

        if ( result is { Data: not null } )
            ProductDetails = result.Data;
    }
}