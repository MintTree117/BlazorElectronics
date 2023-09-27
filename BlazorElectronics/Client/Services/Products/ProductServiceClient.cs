using System.Net.Http.Json;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Client.Services.Products;

public class ProductServiceClient : IProductServiceClient
{
    public event Action? ProductsChanged;
    
    public int PageNumber { get; set; }
    public int ProductCount { get; set; }
    public List<Product_DTO>? Products { get; set; }

    public ProductDetails_DTO? ProductDetails { get; set; }

    const string PRODUCT_SEARCH_URL_BASE = "api/Product/products";
    const string PRODUCT_DETAILS_URL_BASE = "api/Product/details";
    
    readonly HttpClient _http;

    public ProductServiceClient( HttpClient http )
    {
        _http = http;
    }

    public async Task GetProductsTEST( string query )
    {
        //string queryString = $"?filters={Uri.EscapeDataString( Newtonsoft.Json.JsonConvert.SerializeObject( filters ) )}";
        string url = PRODUCT_SEARCH_URL_BASE + query;

        var result = await _http.GetFromJsonAsync<ControllerResponse<Products_DTO>>( url );

        if ( result is { Data: not null } )
        {
            Products = result.Data.Products;
        }

        ProductsChanged?.Invoke();
    }
    public async Task GetProducts( ProductSearchFilters_DTO? filters )
    {
        string queryString = $"?filters={Uri.EscapeDataString( Newtonsoft.Json.JsonConvert.SerializeObject( filters ) )}";
        string url = PRODUCT_SEARCH_URL_BASE + queryString;
        
        var result = await _http.GetFromJsonAsync<ControllerResponse<Products_DTO>>( url );

        if ( result is { Data: not null } ) {
            Products = result.Data.Products;
        }

        ProductsChanged?.Invoke();
    }
    public async Task GetProductDetails( int productId )
    {
        string url = PRODUCT_DETAILS_URL_BASE + $"{productId}";
        
        var result = await _http.GetFromJsonAsync<ControllerResponse<ProductDetails_DTO>>( url );

        if ( result is { Data: not null } )
            ProductDetails = result.Data;
    }
}