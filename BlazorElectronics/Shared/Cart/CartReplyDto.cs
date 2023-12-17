namespace BlazorElectronics.Shared.Cart;

public sealed class CartReplyDto
{
    public List<CartProductDto> Items { get; set; } = new();

    public decimal GetTotalPrice()
    {
        return Items.Sum( item => item.Price * item.Quantity );
    }
    public bool GetSameItem( CartProductDto itemToCompare, out CartProductDto? item )
    {
        item = Items.Find( x => x.ProductId == itemToCompare.ProductId );
        return item != null;
    }
    public void AddOrUpdateQuantity( CartProductDto item )
    {
        if ( GetSameItem( item, out CartProductDto? sameItem ) )
            sameItem!.Quantity += item.Quantity;
        else
            Items.Add( item );
    }
}