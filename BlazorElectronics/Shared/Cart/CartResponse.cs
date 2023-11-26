namespace BlazorElectronics.Shared.Cart;

public sealed class CartResponse
{
    public List<CartProductResponse> Items { get; set; } = new();

    public decimal GetTotalPrice()
    {
        return Items.Sum( item => item.GetRealPrice() * item.Quantity );
    }

    public bool GetSameItem( CartProductResponse itemToCompare, out CartProductResponse? item )
    {
        item = Items.Find( x => x.ProductId == itemToCompare.ProductId && x.VariantId == itemToCompare.VariantId );
        return item != null;
    }

    public void AddOrUpdateQuantity( CartProductResponse item )
    {
        if ( GetSameItem( item, out CartProductResponse? sameItem ) )
            sameItem!.Quantity += item.Quantity;
        else
            Items.Add( item );
    }
}