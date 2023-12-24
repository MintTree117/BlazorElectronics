namespace BlazorElectronics.Shared.Cart;

public sealed class CartItemDto
{
    public CartItemDto()
    {
        
    }
    public CartItemDto( int id, int quantity )
    {
        ProductId = id;
        Quantity = quantity;
    }
    
    public int ProductId { get; init; }
    public int Quantity { get; set; }
    
    public void IncreaseQuantity( int amount )
    {
        Quantity += amount;
    }
}