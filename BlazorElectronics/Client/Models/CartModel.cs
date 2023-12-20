using BlazorElectronics.Shared.Cart;

namespace BlazorElectronics.Client.Models;

public sealed class CartModel
{
    public CartModel()
    {
        
    }
    public CartModel( List<CartProductDto> products )
    {
        Items = products;
    }
    
    public List<CartProductDto> Items { get; set; } = new();

    public List<CartItemDto> GetItemsDto()
    {
        return Items
            .Select( product => new CartItemDto { ProductId = product.ProductId, ItemQuantity = product.ItemQuantity } )
            .ToList();
    }
    
    public CartInfoModel GetCartInfo()
    {
        return new CartInfoModel
        {
            Quantity = Items.Count,
            TotalPrice = GetTotalPrice()
        };
    }
    public void Add( CartProductDto item )
    {
        if ( Items.All( i => i.ProductId != item.ProductId ) )
            Items.Add( item );
    }
    public void Remove( int productId )
    {
        CartProductDto? i = Items.Find( i => i.ProductId == productId );

        if ( i is not null )
            Items.Remove( i );
    }
    public bool GetSameItem( CartProductDto itemToCompare, out CartProductDto? item )
    {
        item = Items.Find( x => x.ProductId == itemToCompare.ProductId );
        return item != null;
    }
    public void AddOrUpdateQuantity( CartProductDto item )
    {
        if ( GetSameItem( item, out CartProductDto? sameItem ) )
            sameItem!.ItemQuantity += item.ItemQuantity;
        else
            Items.Add( item );
    }

    decimal GetTotalPrice()
    {
        return Items.Sum( GetItemPrice );
    }
    decimal GetItemPrice( CartProductDto p )
    {
        return p.SalePrice ?? p.Price;
    }
}