using BlazorElectronics.Shared.Users;

namespace BlazorElectronics.Shared.Cart;

public sealed class CartItemsInsertRequest
{
    public UserRequest? ApiRequest { get; set; }
    public List<CartItemIdsDto> Items { get; set; } = new();
}