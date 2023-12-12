using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Client.Services.Products;

public sealed class ProductServiceClient : ClientService, IProductServiceClient
{
    public ProductServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public async Task<ServiceReply<ProductSearchResponse?>> GetProductSearch( ProductSearchRequest request )
    {
        return await TryPostRequest<ProductSearchResponse>( "api/Products/search", request );
    }
}