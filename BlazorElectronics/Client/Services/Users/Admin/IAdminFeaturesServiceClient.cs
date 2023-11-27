using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Features;

namespace BlazorElectronics.Client.Services.Users.Admin;

public interface IAdminFeaturesServiceClient
{
    Task<ApiReply<FeaturesViewDto?>> GetFeaturesView();
    Task<ApiReply<FeaturedProductEditDto?>> GetFeaturedProductEdit( IntDto dto );
    Task<ApiReply<bool>> AddFeaturedProduct( FeaturedProductEditDto dto );
    Task<ApiReply<bool>> AddFeaturedDeal( IntDto dto );
    Task<ApiReply<bool>> UpdateFeaturedProduct( FeaturedProductEditDto dto );
    Task<ApiReply<bool>> RemoveFeaturedProduct( IntDto dto );
    Task<ApiReply<bool>> RemoveFeaturedDeal( IntDto dto );
}