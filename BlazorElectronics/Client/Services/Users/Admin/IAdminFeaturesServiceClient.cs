using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Admin.Features;

namespace BlazorElectronics.Client.Services.Users.Admin;

public interface IAdminFeaturesServiceClient
{
    Task<ApiReply<FeaturesViewDto?>> GetFeaturesView();
    Task<ApiReply<FeaturedProductEditDto?>> GetFeaturedProductEdit( int productId );
    Task<ApiReply<bool>> AddFeaturedProduct( FeaturedProductEditDto dto );
    Task<ApiReply<bool>> AddFeaturedDeal( IdDto dto );
    Task<ApiReply<bool>> UpdateFeaturedProduct( FeaturedProductEditDto dto );
    Task<ApiReply<bool>> RemoveFeaturedProduct( IdDto dto );
    Task<ApiReply<bool>> RemoveFeaturedDeal( IdDto dto );
}