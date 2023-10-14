using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorElectronics.Server.Models.Products;

public class ProductAndVariants
{
    public int ProductId_ProductsUnion { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductThumbnail { get; set; } = string.Empty;
    public int ProductRating { get; set; }
    public List<ProductVariant> ProductVariants { get; set; } = new();
    public int ProductVariantId { get; set; }
    public int VariantId { get; set; }
    public string? VariantName { get; set; }
    [Column( TypeName = "decimal(18,2)" )] public decimal VariantPriceMain { get; set; }
    [Column( TypeName = "decimal(18,2)" )] public decimal VariantPriceSale { get; set; }
}