using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorElectronics.Shared.DataTransferObjects.Products;

public sealed class ProductAndVariantsDto
{
    public int ProductId { get; set; } 
    //public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductThumbnail { get; set; } = string.Empty;
    public int ProductRating { get; set; }
    public List<ProductVariant_DTO> ProductVariants { get; set; } = new();
    public int ProductVariantId { get; set; }
    public int VariantId_VariantsUnion { get; set; }
    public string? VariantName { get; set; }
    [Column( TypeName = "decimal(18,2)" )] public decimal VariantPriceMain { get; set; }
    [Column( TypeName = "decimal(18,2)" )] public decimal VariantPriceSale { get; set; }
}