namespace BlazorElectronics.Shared.Features;

public class FeaturedDealDto
{
    public int ProductId { get; protected set; }
    public string ProductName { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public decimal SalePrice { get; init; }
    public string Image { get; set; } = string.Empty;
}