using BlazorElectronics.Shared.Inbound.Users;

namespace BlazorElectronics.Shared.Inbound.Cart;

public sealed class CartItemsInsertRequest
{
    public UserRequest? ApiRequest { get; set; }
    public List<CartItemId_DTO> Items { get; set; } = new();
}