using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Server.Services.Features;

public interface IFeaturesService
{
    Task<ApiReply<FeaturedProductsResponse?>> GetFeaturedProducts();
    Task<ApiReply<FeaturedDealsResponse?>> GetFeaturedDeals();
}