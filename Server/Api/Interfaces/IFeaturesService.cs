using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface IFeaturesService
{
    Task<ServiceReply<List<FeatureDealDto>?>> GetDeals( int rows, int page );
    Task<ServiceReply<List<FeatureDto>?>> GetFeatures();
    Task<ServiceReply<List<CrudViewDto>?>> GetFeaturesView();
    Task<ServiceReply<FeatureDtoEditDto?>> GetFeatureEdit( int featureId );
    Task<ServiceReply<int>> AddFeature( FeatureDtoEditDto dto );
    Task<ServiceReply<bool>> UpdateFeature( FeatureDtoEditDto dto );
    Task<ServiceReply<bool>> RemoveFeature( int featureId );
}