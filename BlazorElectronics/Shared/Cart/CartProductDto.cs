using BlazorElectronics.Shared.Products;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Shared.Cart;

public sealed class CartProductDto
{
    public CartProductDto()
    {
        
    }
    public CartProductDto( ProductDto p )
    {
        ProductId = p.Id;
        Title = p.Title;
        Thumbnail = p.Thumbnail;
        Price = p.Price;
        SalePrice = p.SalePrice;
    }
    public CartProductDto( ProductSummaryDto p )
    {
        ProductId = p.Id;
        Title = p.Title;
        Thumbnail = p.Thumbnail;
        Price = p.Price;
        SalePrice = p.SalePrice;
    }
    
    public int ProductId { get; set; }
    public string Title { get; init; } = string.Empty;
    public string Thumbnail { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public decimal? SalePrice { get; init; }
    public int ItemQuantity { get; set; } = 1;
}