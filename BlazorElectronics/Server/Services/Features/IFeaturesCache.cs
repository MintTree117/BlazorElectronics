using BlazorElectronics.Shared.Outbound.Features;

namespace BlazorElectronics.Server.Services.Features;

public interface IFeaturesCache
{
    Task<FeaturedProducts_DTO?> GetFeaturedProducts();
    Task<FeaturedDeals_DTO?> GetFeaturedDeals();
    Task CacheFeaturedProducts( FeaturedProducts_DTO dto );
    Task CacheFeaturedDeals( FeaturedDeals_DTO dto );
}