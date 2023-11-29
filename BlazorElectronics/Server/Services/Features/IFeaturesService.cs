using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Server.Services.Features;

public interface IFeaturesService
{
    Task<ApiReply<FeaturesResponse?>> GetFeatures();
    Task<ApiReply<FeaturesResponse?>> GetView();
    Task<ApiReply<bool>> AddFeaturedProduct( FeaturedProductDto dto );
    Task<ApiReply<bool>> AddFeaturedDeal( int productId );
    Task<ApiReply<FeaturedProductDto?>> GetFeaturedProductEdit( int productId );
    Task<ApiReply<bool>> UpdateFeaturedProduct( FeaturedProductDto dto );
    Task<ApiReply<bool>> RemoveFeaturedProduct( int productId );
    Task<ApiReply<bool>> RemoveFeaturedDeal( int productId );
}