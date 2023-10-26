using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Outbound.Features;

namespace BlazorElectronics.Client.Services.Features;

public interface IFeaturesServiceClient
{
    Task<ServiceResponse<FeaturedProducts_DTO?>?> GetFeaturedProducts();
    Task<ServiceResponse<FeaturedDeals_DTO?>?> GetFeaturedDeals();
}