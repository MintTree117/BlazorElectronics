using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface IFeaturesRepository
{
    Task<IEnumerable<FeatureDealDto>?> GetDeals( int rows, int offset );
    Task<IEnumerable<FeatureDto>?> GetFeatures();
    Task<FeatureDtoEditDto?> GetFeatureEdit( int featureId );
    Task<int> InsertFeature( FeatureDtoEditDto dto );
    Task<bool> UpdateFeature( FeatureDtoEditDto dto );
    Task<bool> DeleteFeature( int featureId );
}