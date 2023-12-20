using BlazorElectronics.Server.Core.Models.Promos;

namespace BlazorElectronics.Server.Core.Interfaces;

public interface IPromoRepository
{
    Task<IEnumerable<PromoModel>?> GetView();
    Task<PromoModel?> GetEdit( int promoId );
    Task<int> Insert( PromoModel promo );
    Task<bool> Update( PromoModel promo );
    Task<bool> Delete( int promoId );
}