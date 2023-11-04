using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Outbound.Features;

namespace BlazorElectronics.Server.Services.Features;

public interface IFeaturesService
{
    Task<Reply<FeaturedProducts_DTO?>> GetFeaturedProducts();
    Task<Reply<FeaturedDeals_DTO?>> GetFeaturedDeals();
}