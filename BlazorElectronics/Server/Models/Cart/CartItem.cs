namespace BlazorElectronics.Server.Models.Cart;

public sealed class CartItem
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int VariantId { get; set; }
    public int Quantity { get; set; } = 1;
}