using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Server.Repositories.Features;

public interface IFeaturesRepository
{
    Task<FeaturesResponse?> GetView();
    Task<bool> InsertFeaturedProduct( FeaturedProductDto dto );
    Task<bool> InsertFeaturedDeal( int productId );
    Task<FeaturedProductDto?> GetFeaturedProductEdit( int productId );
    Task<bool> UpdateFeaturedProduct( FeaturedProductDto dto );
    Task<bool> DeleteFeaturedProduct( int productId );
    Task<bool> DeleteFeaturedDeal( int productId );
}