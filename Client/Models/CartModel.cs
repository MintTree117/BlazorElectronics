using BlazorElectronics.Shared.Cart;
using BlazorElectronics.Shared.Promos;

namespace BlazorElectronics.Client.Models;

public sealed class CartModel
{
    public CartModel()
    {
        
    }
    public CartModel( CartDto dto )
    {
        Products = dto.Products;
        PromoCodes = dto.PromoCodes;
    }
    
    public List<CartProductDto> Products { get; set; } = new();
    public List<PromoCodeDto> PromoCodes { get; set; } = new();
    
    public List<CartItemDto> GetItemsDto()
    {
        return Products
            .Select( product => new CartItemDto { ProductId = product.ProductId, Quantity = product.Quantity } )
            .ToList();
    }
    public CartInfoModel GetCartInfo()
    {
        return new CartInfoModel
        {
            Quantity = Products.Count,
            TotalPrice = GetTotalPrice()
        };
    }
    
    public void AddItem( CartProductDto item )
    {
        CartProductDto? product = Products.Find( p => p.ProductId == item.ProductId );

        if ( product is null )
            Products.Add( item );
        else
            product.Quantity += item.Quantity;
    }
    public void RemoveItem( int productId )
    {
        CartProductDto? i = Products.Find( i => i.ProductId == productId );

        if ( i is not null )
            Products.Remove( i );
    }
    public bool GetSameItem( CartProductDto itemToCompare, out CartProductDto? item )
    {
        item = Products.Find( x => x.ProductId == itemToCompare.ProductId );
        return item != null;
    }
    public void AddOrUpdateQuantity( CartProductDto item )
    {
        if ( GetSameItem( item, out CartProductDto? sameItem ) )
            sameItem!.Quantity += item.Quantity;
        else
            Products.Add( item );
    }

    public void AddPromo( PromoCodeDto promo )
    {
        if ( PromoCodes.All( p => p.PromoId != promo.PromoId ) )
            PromoCodes.Add( promo );
    }
    public void RemovePromo( int id )
    {
        PromoCodeDto? p = PromoCodes.Find( p => p.PromoId == id );

        if ( p is not null )
            PromoCodes.Remove( p );
    }
    
    public decimal GetItemPrice( CartProductDto p )
    {
        return p.SalePrice ?? p.Price;
    }
    public decimal GetTotalPrice()
    {
        return Products.Sum( GetItemPrice );
    }
    public decimal CalculateTotalTax( double percent )
    {
        return GetTotalPrice() * ( decimal ) percent;
    }
    public decimal CaluclateFinalPrice( double tax )
    {
        return GetTotalPrice() + CalculateTotalTax( tax );
    }
}