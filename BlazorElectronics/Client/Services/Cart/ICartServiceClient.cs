using BlazorElectronics.Client.Models;
using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Cart;

namespace BlazorElectronics.Client.Services.Cart;

public interface ICartServiceClient
{
    public event Action<CartInfoModel>? OnChange;
    
    Task<ServiceReply<CartInfoModel>> GetLocalCartInfo();
    Task<ServiceReply<CartModel?>> UpdateCart();
    Task<ServiceReply<int>> HasItem( int productId );
    Task<ServiceReply<bool>> AddOrUpdateItem( CartProductDto product );
    Task<ServiceReply<bool>> RemoveItem( int productId );
    Task<ServiceReply<bool>> ClearCart();

    Task<ServiceReply<bool>> AddPromoCode( string code );
    Task<ServiceReply<bool>> RemovePromoCode( string code );
}