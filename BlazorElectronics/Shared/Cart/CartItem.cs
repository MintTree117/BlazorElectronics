namespace BlazorElectronics.Shared.Cart;

public sealed class CartItem
{
    public int ProductId { get; init; }
    public int Quantity { get; set; }
}