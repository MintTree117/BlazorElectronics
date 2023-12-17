using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface IFeaturesRepository
{
    Task<FeaturesReplyDto?> Get();
    Task<IEnumerable<FeatureDto>?> GetFeatures();
    Task<IEnumerable<FeaturedDealDto>?> GetDeals();
    Task<FeatureDtoEditDto?> GetFeature( int featureId );
    Task<FeaturedDealDtoEditDto?> GetDeal( int productId );
    Task<int> InsertFeature( FeatureDtoEditDto dto );
    Task<int> InsertDeal( FeaturedDealDtoEditDto dto );
    Task<bool> UpdateFeature( FeatureDtoEditDto dto );
    Task<bool> UpdateDeal( FeaturedDealDtoEditDto dto );
    Task<bool> DeleteFeature( int featureId );
    Task<bool> DeleteDeal( int productId );
}