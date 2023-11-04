using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Outbound.Features;

namespace BlazorElectronics.Client.Services.Features;

public interface IFeaturesServiceClient
{
    Task<Reply<FeaturedProducts_DTO?>?> GetFeaturedProducts();
    Task<Reply<FeaturedDeals_DTO?>?> GetFeaturedDeals();
}