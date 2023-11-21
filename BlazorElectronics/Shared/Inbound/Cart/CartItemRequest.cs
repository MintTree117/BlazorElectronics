using BlazorElectronics.Shared.Inbound.Users;

namespace BlazorElectronics.Shared.Inbound.Cart;

public sealed class CartItemRequest
{
    public UserApiRequest? ApiRequest { get; set; }
    public CartItemId_DTO? CartItemIds { get; set; }
}