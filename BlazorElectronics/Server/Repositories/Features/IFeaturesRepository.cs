using BlazorElectronics.Server.Models.Features;
using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Server.Repositories.Features;

public interface IFeaturesRepository
{
    Task<FeaturesModel?> Get();
    Task<IEnumerable<Feature>?> GetFeatures();
    Task<IEnumerable<FeaturedDeal>?> GetDeals();
    Task<Feature?> GetFeature( int featureId );
    Task<FeaturedDeal?> GetDeal( int productId );
    Task<Feature?> InsertFeature( Feature dto );
    Task<FeaturedDeal?> InsertDeal( FeaturedDeal dto );
    Task<bool> UpdateFeature( Feature dto );
    Task<bool> UpdateDeal( FeaturedDeal dto );
    Task<bool> DeleteFeature( int featureId );
    Task<bool> DeleteDeal( int productId );
}