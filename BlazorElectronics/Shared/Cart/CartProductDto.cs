namespace BlazorElectronics.Shared.Cart;

public sealed class CartProductDto
{
    public int UserId { get; init; }
    public int ProductId { get; init; }
    public string ProductTitle { get; init; } = string.Empty;
    public string ProductThumbnail { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public decimal? SalePrice { get; init; }
    public int Quantity { get; set; } = 1;
}