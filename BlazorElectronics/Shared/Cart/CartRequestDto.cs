namespace BlazorElectronics.Shared.Cart;

public sealed class CartRequestDto
{
    public CartRequestDto()
    {
        
    }
    public CartRequestDto( CartReplyDto dto )
    {
        foreach ( CartProductDto d in dto.Items )
        {
            Items.Add( new CartItemDto
            {
                ProductId = d.ProductId,
                Quantity = d.Quantity
            } );
        }
    }
    
    public List<CartItemDto> Items { get; init; } = new();
}