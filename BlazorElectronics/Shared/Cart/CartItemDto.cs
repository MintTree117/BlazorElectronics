namespace BlazorElectronics.Shared.Cart;

public sealed class CartItemDto
{
    public int ProductId { get; init; }
    public int Quantity { get; set; }
    
    public void IncreaseQuantity( int amount )
    {
        Quantity += amount;
    }
}