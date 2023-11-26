using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Client.Services.Features;

public interface IFeaturesServiceClient
{
    Task<ApiReply<FeaturedProductsResponse?>?> GetFeaturedProducts();
    Task<ApiReply<FeaturedDealsResponse?>?> GetFeaturedDeals();
}