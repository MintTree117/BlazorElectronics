namespace BlazorElectronics.Shared.Cart;

public sealed class CartRequest
{
    public List<CartItem> Items { get; init; } = new();
}