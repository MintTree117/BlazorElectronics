namespace BlazorElectronics.Shared.Cart;

public sealed class CartItemDto
{
    public CartItemDto()
    {
        
    }
    public CartItemDto( int id, int quantity )
    {
        ProductId = id;
        ItemQuantity = quantity;
    }
    
    public int ProductId { get; init; }
    public int ItemQuantity { get; set; }
    
    public void IncreaseQuantity( int amount )
    {
        ItemQuantity += amount;
    }
}