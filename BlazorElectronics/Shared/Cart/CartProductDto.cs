namespace BlazorElectronics.Shared.Cart;

public sealed class CartProductDto
{
    public int UserId { get; init; }
    public int ProductId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Thumbnail { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public decimal? SalePrice { get; init; }
    public int ItemQuantity { get; set; } = 1;
}