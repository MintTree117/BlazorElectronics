using BlazorElectronics.Server.Models.Features;

namespace BlazorElectronics.Server.Repositories.Features;

public interface IFeaturesRepository
{
    Task<IEnumerable<FeaturedProduct>?> GetFeaturedProducts();
    Task<IEnumerable<FeaturedDeal>?> GetFeaturedDeals();
}