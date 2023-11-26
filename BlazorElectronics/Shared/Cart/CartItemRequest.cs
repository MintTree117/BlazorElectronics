using BlazorElectronics.Shared.Users;

namespace BlazorElectronics.Shared.Cart;

public sealed class CartItemRequest
{
    public UserRequest? ApiRequest { get; set; }
    public CartItemIdsDto? CartItemIds { get; set; }
}