namespace BlazorElectronics.Shared.Features;

public sealed class FeaturedDeal_DTO
{
    public int ProductId { get; set; }
    public int VariantId { get; set; }
    public string ProductTitle { get; set; } = string.Empty;
    public string ProductThumbnail { get; set; } = string.Empty;
    public float ProductRating { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal SalePrice { get; set; }
}