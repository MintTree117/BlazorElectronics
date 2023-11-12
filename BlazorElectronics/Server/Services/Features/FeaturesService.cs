using BlazorElectronics.Server.Models.Features;
using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Server.Repositories.Features;
using BlazorElectronics.Shared.Outbound.Features;

namespace BlazorElectronics.Server.Services.Features;

public class FeaturesService : IFeaturesService
{
    readonly IFeaturesCache _cache;
    public Task<ApiReply<FeaturedProductsResponse?>> GetFeaturedProducts()
    {
        throw new NotImplementedException();
    }
    public Task<ApiReply<FeaturedDealsResponse?>> GetFeaturedDeals()
    {
        throw new NotImplementedException();
    }
}