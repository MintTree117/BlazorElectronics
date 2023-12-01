using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Client.Services.Users.Admin;

public interface IAdminFeaturesServiceClient
{
    Task<ServiceReply<FeaturesResponse?>> GetFeaturesView();
    Task<ServiceReply<FeaturedProductDto?>> GetFeaturedProductEdit( IntDto dto );
    Task<ServiceReply<bool>> AddFeaturedProduct( FeaturedProductDto dto );
    Task<ServiceReply<bool>> AddFeaturedDeal( IntDto dto );
    Task<ServiceReply<bool>> UpdateFeaturedProduct( FeaturedProductDto dto );
    Task<ServiceReply<bool>> RemoveFeaturedProduct( IntDto dto );
    Task<ServiceReply<bool>> RemoveFeaturedDeal( IntDto dto );
}