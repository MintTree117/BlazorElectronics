using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Server.Services.Features;

public interface IFeaturesService
{
    Task<ServiceReply<FeaturesResponse?>> GetFeatures();
    Task<ServiceReply<List<AdminItemViewDto>?>> GetFeaturesView();
    Task<ServiceReply<List<AdminItemViewDto>?>> GetDealsView();
    Task<ServiceReply<Feature?>> GetFeature( int featureId );
    Task<ServiceReply<FeaturedDeal?>> GetDeal( int productId );
    Task<ServiceReply<Feature?>> AddFeature( Feature dto );
    Task<ServiceReply<FeaturedDeal?>> AddDeal( FeaturedDeal dto );
    Task<ServiceReply<bool>> UpdateFeature( Feature dto );
    Task<ServiceReply<bool>> UpdateDeal( FeaturedDeal dto );
    Task<ServiceReply<bool>> RemoveFeature( int featureId );
    Task<ServiceReply<bool>> RemoveDeal( int productId );
}