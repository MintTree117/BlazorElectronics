using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Outbound.Features;

namespace BlazorElectronics.Server.Services.Features;

public interface IFeaturesService
{
    Task<ServiceResponse<FeaturedProducts_DTO?>> GetFeaturedProducts();
    Task<ServiceResponse<FeaturedDeals_DTO?>> GetFeaturedDeals();
}