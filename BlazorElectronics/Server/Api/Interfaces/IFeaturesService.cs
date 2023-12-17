using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface IFeaturesService
{
    Task<ServiceReply<FeaturesReplyDto?>> GetFeatures();
    Task<ServiceReply<List<CrudViewDto>?>> GetFeaturesView();
    Task<ServiceReply<List<CrudViewDto>?>> GetDealsView();
    Task<ServiceReply<FeatureDtoEditDto?>> GetFeatureEdit( int featureId );
    Task<ServiceReply<FeaturedDealDtoEditDto?>> GetDealEdit( int productId );
    Task<ServiceReply<int>> AddFeature( FeatureDtoEditDto dto );
    Task<ServiceReply<int>> AddDeal( FeaturedDealDtoEditDto dto );
    Task<ServiceReply<bool>> UpdateFeature( FeatureDtoEditDto dto );
    Task<ServiceReply<bool>> UpdateDeal( FeaturedDealDtoEditDto dto );
    Task<ServiceReply<bool>> RemoveFeature( int featureId );
    Task<ServiceReply<bool>> RemoveDeal( int productId );
}