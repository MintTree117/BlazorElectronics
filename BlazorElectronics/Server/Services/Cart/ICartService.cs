using BlazorElectronics.Shared;
using BlazorElectronics.Shared.Inbound.Cart;
using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Server.Services.Cart;

public interface ICartService
{
    Task<ServiceResponse<Cart_DTO?>> GetOrUpdateCartItems( CartItemIds clientItems );
}