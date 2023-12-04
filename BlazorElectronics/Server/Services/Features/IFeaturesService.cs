using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Server.Services.Features;

public interface IFeaturesService
{
    Task<ServiceReply<FeaturesResponse?>> GetFeatures();
    Task<ServiceReply<List<CrudView>?>> GetFeaturesView();
    Task<ServiceReply<List<CrudView>?>> GetDealsView();
    Task<ServiceReply<FeatureEdit?>> GetFeatureEdit( int featureId );
    Task<ServiceReply<FeaturedDealEdit?>> GetDealEdit( int productId );
    Task<ServiceReply<int>> AddFeature( FeatureEdit dto );
    Task<ServiceReply<int>> AddDeal( FeaturedDealEdit dto );
    Task<ServiceReply<bool>> UpdateFeature( FeatureEdit dto );
    Task<ServiceReply<bool>> UpdateDeal( FeaturedDealEdit dto );
    Task<ServiceReply<bool>> RemoveFeature( int featureId );
    Task<ServiceReply<bool>> RemoveDeal( int productId );
}