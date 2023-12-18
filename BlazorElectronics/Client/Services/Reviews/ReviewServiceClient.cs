using Blazored.LocalStorage;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Reviews;

namespace BlazorElectronics.Client.Services.Reviews;

public sealed class ReviewServiceClient : ClientService, IReviewServiceClient
{
    const string API_ROUTE = "api/Reviews";
    const string API_ROUTE_PRODUCT = $"{API_ROUTE}/for-product";
    
    public ReviewServiceClient( ILogger<ClientService> logger, HttpClient http, ILocalStorageService storage )
        : base( logger, http, storage ) { }
    
    public async Task<ServiceReply<ProductReviewsReplyDto?>> GetForProduct( ProductReviewsGetDto dto )
    {
        return await TryPostRequest<ProductReviewsReplyDto?>( API_ROUTE_PRODUCT, dto );
    }
}