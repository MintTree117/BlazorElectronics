using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Server.Services.Features;

public interface IFeaturesService
{
    Task<ServiceReply<FeaturesResponse?>> GetFeatures();
    Task<ServiceReply<FeaturesResponse?>> GetView();
    Task<ServiceReply<bool>> AddFeaturedProduct( FeaturedProductDto dto );
    Task<ServiceReply<bool>> AddFeaturedDeal( int productId );
    Task<ServiceReply<FeaturedProductDto?>> GetFeaturedProductEdit( int productId );
    Task<ServiceReply<bool>> UpdateFeaturedProduct( FeaturedProductDto dto );
    Task<ServiceReply<bool>> RemoveFeaturedProduct( int productId );
    Task<ServiceReply<bool>> RemoveFeaturedDeal( int productId );
}