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
        Dictionary<string, object> parameters = new()
        {
            { nameof( dto.TotalMatches ), dto.TotalMatches },
            { nameof( dto.ProductId ), dto.ProductId },
            { nameof( dto.Rows ), dto.Rows },
            { nameof( dto.Page ), dto.Page },
            { nameof( dto.SortType ), dto.SortType }
        };

        return await TryGetRequest<ProductReviewsReplyDto?>( API_ROUTE_PRODUCT, parameters );
    }
}