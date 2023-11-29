using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Client.Services.Users.Admin;

public interface IAdminFeaturesServiceClient
{
    Task<ApiReply<FeaturesResponse?>> GetFeaturesView();
    Task<ApiReply<FeaturedProductDto?>> GetFeaturedProductEdit( IntDto dto );
    Task<ApiReply<bool>> AddFeaturedProduct( FeaturedProductDto dto );
    Task<ApiReply<bool>> AddFeaturedDeal( IntDto dto );
    Task<ApiReply<bool>> UpdateFeaturedProduct( FeaturedProductDto dto );
    Task<ApiReply<bool>> RemoveFeaturedProduct( IntDto dto );
    Task<ApiReply<bool>> RemoveFeaturedDeal( IntDto dto );
}