using BlazorElectronics.Client.Models;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Cart;
using BlazorElectronics.Shared.Promos;

namespace BlazorElectronics.Client.Services.Cart;

public interface ICartServiceClient
{
    public event Action<CartInfoModel>? OnChange;
    
    Task<ServiceReply<CartInfoModel>> GetLocalCartInfo();
    Task<ServiceReply<CartModel?>> UpdateCart();
    Task<ServiceReply<int>> HasItem( int productId );
    Task<ServiceReply<CartModel?>> AddOrUpdateItem( CartProductDto product );
    Task<ServiceReply<CartModel?>> RemoveItem( int productId );
    Task<ServiceReply<bool>> ClearCart();

    Task<ServiceReply<PromoCodeDto?>> AddPromoCode( string code );
    Task<ServiceReply<bool>> RemovePromoCode( int id );
}