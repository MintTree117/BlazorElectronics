using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface IFeaturesRepository
{
    Task<FeaturesResponse?> Get();
    Task<IEnumerable<Feature>?> GetFeatures();
    Task<IEnumerable<FeaturedDeal>?> GetDeals();
    Task<FeatureEdit?> GetFeature( int featureId );
    Task<FeaturedDealEdit?> GetDeal( int productId );
    Task<int> InsertFeature( FeatureEdit dto );
    Task<int> InsertDeal( FeaturedDealEdit dto );
    Task<bool> UpdateFeature( FeatureEdit dto );
    Task<bool> UpdateDeal( FeaturedDealEdit dto );
    Task<bool> DeleteFeature( int featureId );
    Task<bool> DeleteDeal( int productId );
}