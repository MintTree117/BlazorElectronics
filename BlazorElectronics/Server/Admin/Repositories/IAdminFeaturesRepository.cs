using BlazorElectronics.Shared.Admin.Features;

namespace BlazorElectronics.Server.Admin.Repositories;

public interface IAdminFeaturesRepository
{
    Task<FeaturesViewDto?> GetView();
    Task<bool> InsertFeaturedProduct( FeaturedProductEditDto dto );
    Task<bool> InsertFeaturedDeal( FeaturedDealEditDto dto );
    Task<bool> UpdateFeaturedProduct( FeaturedProductEditDto dto );
    Task<bool> DeleteFeaturedProduct( int id );
    Task<bool> DeleteFeaturedDeal( int id );
}