using BlazorElectronics.Server.Models.Features;
using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Server.Repositories.Features;
using BlazorElectronics.Shared.Outbound.Features;

namespace BlazorElectronics.Server.Services.Features;

public class FeaturesService : IFeaturesService
{
    readonly IFeaturesCache _cache;
    public Task<Reply<FeaturedProducts_DTO?>> GetFeaturedProducts()
    {
        throw new NotImplementedException();
    }
    public Task<Reply<FeaturedDeals_DTO?>> GetFeaturedDeals()
    {
        throw new NotImplementedException();
    }
}