namespace BlazorElectronics.Shared.Cart;

public sealed class CartProductResponse
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public string ProductTitle { get; set; } = string.Empty;
    public string ProductThumbnail { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public int Quantity { get; set; } = 1;
}