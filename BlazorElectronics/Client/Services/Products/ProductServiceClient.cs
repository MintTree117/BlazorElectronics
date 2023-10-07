using System.Net.Http.Json;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Client.Services.Products;

public class ProductServiceClient : IProductServiceClient
{
    public event Action<ServiceResponse<ProductSearch_DTO?>>? ProductSearchChanged;
    
    const string SERVER_RESPONSE_MESSAGE_NULL = "Server returned null!";
    const string SERVER_RESPONSE_MESSAGE_FAILURE_NO_MESSAGE = "Server returned null data without a return message!";
    const string PRODUCT_SEARCH_URL_BASE = "api/Product/products";
    const string PRODUCT_DETAILS_URL_BASE = "api/Product/details/";
    
    readonly HttpClient _http;

    public ProductServiceClient( HttpClient http )
    {
        _http = http;
    }
    
    public async Task SearchProducts( ProductSearchFilters_DTO? filters )
    {
        string queryString = $"?filters={Uri.EscapeDataString( Newtonsoft.Json.JsonConvert.SerializeObject( filters ) )}";
        string url = PRODUCT_SEARCH_URL_BASE + queryString;
        
        var result = await _http.GetFromJsonAsync<ServiceResponse<ProductSearch_DTO?>>( url );

        if ( result == null )
        {
            ProductSearchChanged?.Invoke( new ServiceResponse<ProductSearch_DTO?>( null, false, SERVER_RESPONSE_MESSAGE_NULL ) );
            return;
        }

        if ( result.Data == null || !result.Success )
        {
            ProductSearchChanged?.Invoke( new ServiceResponse<ProductSearch_DTO?>( null, false, result.Message ?? SERVER_RESPONSE_MESSAGE_FAILURE_NO_MESSAGE ) );
            return;
        }

        ProductSearchChanged?.Invoke( result );
    }
    public async Task<ServiceResponse<ProductDetails_DTO?>> GetProductDetails( int productId )
    {
        string url = $"{PRODUCT_DETAILS_URL_BASE}{productId}";

        var serverResponse = await _http.GetFromJsonAsync<ServiceResponse<ProductDetails_DTO?>>( url );
        return serverResponse ?? new ServiceResponse<ProductDetails_DTO?>( null, false, SERVER_RESPONSE_MESSAGE_NULL );
    }
}