namespace BlazorElectronics.Server.Models.Products;

public sealed class ProductSale
{
    public int ProductId { get; set; }
    public int VariantId { get; set; }
    public string ProductTitle { get; set; } = string.Empty;
    public string ProductThumbnail { get; set; } = string.Empty;
    public decimal ProductRating { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal SalePrice { get; set; }
}