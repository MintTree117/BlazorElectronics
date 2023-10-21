using BlazorElectronics.Shared.Outbound.Features;

namespace BlazorElectronics.Server.Services.Features;

public interface IFeaturesCache
{
    Task<FeaturedProducts_DTO?> GetFeaturedProducts();
    Task<FeaturesDeals_DTO?> GetFeaturedDeals();
    Task CacheFeaturedProducts( FeaturedProducts_DTO dto );
    Task CacheFeaturedDeals( FeaturesDeals_DTO dto );
}