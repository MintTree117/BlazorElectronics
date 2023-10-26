namespace BlazorElectronics.Shared.Mutual;

public sealed class Cart_DTO
{
    public List<CartItem_DTO> Items { get; set; } = new();

    public decimal GetTotalPrice()
    {
        return Items.Sum( item => item.GetRealPrice() * item.Quantity );
    }

    public bool GetSameItem( CartItem_DTO itemToCompare, out CartItem_DTO? item )
    {
        item = Items.Find( x => x.ProductId == itemToCompare.ProductId && x.VariantId == itemToCompare.VariantId );
        return item != null;
    }

    public void AddOrUpdateQuantity( CartItem_DTO item )
    {
        if ( GetSameItem( item, out CartItem_DTO? sameItem ) )
            sameItem!.Quantity += item.Quantity;
        else
            Items.Add( item );
    }
}