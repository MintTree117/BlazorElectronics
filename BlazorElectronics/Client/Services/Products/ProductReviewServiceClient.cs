using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.ProductReviews;

namespace BlazorElectronics.Client.Services.Products;

public sealed class ProductReviewServiceClient : ClientService, IProductReviewServiceClient
{
    const string API_ROUTE = "api/ProductReviews";
    const string API_ROUTE_PRODUCT = $"{API_ROUTE}/for-product";
    
    public ProductReviewServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public async Task<ServiceReply<List<ProductReviewDto>?>> GetForProduct( GetProductReviewsDto dto )
    {
        return await TryPostRequest<List<ProductReviewDto>?>( API_ROUTE_PRODUCT, dto );
    }
}