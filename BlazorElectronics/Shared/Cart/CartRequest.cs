namespace BlazorElectronics.Shared.Cart;

public sealed class CartRequest
{
    public List<CartItemDto> Items { get; set; } = new();
}