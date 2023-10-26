using BlazorElectronics.Shared.Inbound.Users;

namespace BlazorElectronics.Shared.Inbound.Cart;

public sealed class CartItemRequest
{
    public SessionApiRequest? ApiRequest { get; set; }
    public CartItemId_DTO? CartItemIds { get; set; }
}