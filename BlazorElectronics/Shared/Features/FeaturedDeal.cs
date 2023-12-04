namespace BlazorElectronics.Shared.Features;

public class FeaturedDeal
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal SalePrice { get; set; }
    public string Image { get; set; } = string.Empty;
}