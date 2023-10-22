namespace BlazorElectronics.Shared.Inbound.Cart;

public sealed class CartItemId
{
    public int ProductId { get; set; }
    public int VariantId { get; set; }
    public int Quantity { get; set; }
}