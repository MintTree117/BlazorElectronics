using BlazorElectronics.Shared.Outbound.Features;

namespace BlazorElectronics.Server.Services.Features;

public interface IFeaturesCache
{
    Task<FeaturedProductsResponse?> GetFeaturedProducts();
    Task<FeaturedDealsResponse?> GetFeaturedDeals();
    Task CacheFeaturedProducts( FeaturedProductsResponse dto );
    Task CacheFeaturedDeals( FeaturedDealsResponse dto );
}