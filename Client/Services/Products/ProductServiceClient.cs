using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Client.Services.Products;

public sealed class ProductServiceClient : ClientService, IProductServiceClient
{
    const string API_ROUTE = "api/Products";
    const string API_ROUTE_SUGGESTIONS = $"{API_ROUTE}/suggestions";
    const string API_ROUTE_SEARCH = $"{API_ROUTE}/search";
    const string API_ROUTE_DETAILS = $"{API_ROUTE}/details";
    
    public ProductServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }

    public async Task<ServiceReply<List<string>?>> GetProductSearchSuggestions( string searchText )
    {
        Dictionary<string, object> p = new() { { "searchText", searchText } };
        return await TryGetRequest<List<string>?>( API_ROUTE_SUGGESTIONS, p );
    }
    public async Task<ServiceReply<ProductSearchReplyDto?>> GetProductSearch( ProductSearchRequestDto requestDto )
    {
        return await TryPostRequest<ProductSearchReplyDto?>( API_ROUTE_SEARCH, requestDto );
    }
    public async Task<ServiceReply<ProductDto?>> GetProductDetails( int productId )
    {
        Dictionary<string, object> p = new() { { "productId", productId } };
        return await TryGetRequest<ProductDto?>( API_ROUTE_DETAILS, p );
    }
}