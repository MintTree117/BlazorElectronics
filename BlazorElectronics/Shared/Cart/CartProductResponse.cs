namespace BlazorElectronics.Shared.Cart;

public sealed class CartProductResponse
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public string? ProductTitle { get; set; }
    public string? ProductThumbnail { get; set; }
    public int VariantId { get; set; }
    public string? VariantName { get; set; }
    public decimal MainPrice { get; set; }
    public decimal? SalePrice { get; set; }
    public int Quantity { get; set; } = 1;

    public decimal GetRealPrice()
    {
        return HasSale() ? SalePrice!.Value : MainPrice;
    }

    public bool HasSale()
    {
        return SalePrice != null && SalePrice.Value < MainPrice;
    }
}