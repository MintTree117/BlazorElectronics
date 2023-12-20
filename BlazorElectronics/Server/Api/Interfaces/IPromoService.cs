using BlazorElectronics.Shared.Promos;

namespace BlazorElectronics.Server.Api.Interfaces;

public interface IPromoService
{
    Task<ServiceReply<List<PromoEditDto>?>> GetView();
    Task<ServiceReply<PromoEditDto?>> GetEdit( int promoId );
    Task<ServiceReply<int>> Add( PromoEditDto dto );
    Task<ServiceReply<bool>> Update( PromoEditDto dto );
    Task<ServiceReply<bool>> Remove( int promoId );
}