using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Outbound.Features;

namespace BlazorElectronics.Client.Services.Features;

public interface IFeaturesServiceClient
{
    Task<ApiReply<FeaturedProductsResponse?>?> GetFeaturedProducts();
    Task<ApiReply<FeaturedDealsResponse?>?> GetFeaturedDeals();
}