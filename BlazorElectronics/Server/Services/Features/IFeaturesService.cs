using BlazorElectronics.Shared.Admin.Features;

namespace BlazorElectronics.Server.Services.Features;

public interface IFeaturesService
{
    Task<ApiReply<FeaturesResponse?>> GetFeatures();
    Task<ApiReply<FeaturesResponse?>> GetView();
    Task<ApiReply<bool>> AddFeaturedProduct( FeaturedProductEditDto dto );
    Task<ApiReply<bool>> AddFeaturedDeal( int productId );
    Task<ApiReply<FeaturedProductEditDto?>> GetFeaturedProductEdit( int productId );
    Task<ApiReply<bool>> UpdateFeaturedProduct( FeaturedProductEditDto dto );
    Task<ApiReply<bool>> RemoveFeaturedProduct( int productId );
    Task<ApiReply<bool>> RemoveFeaturedDeal( int productId );
}