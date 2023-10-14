using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorElectronics.Server.Models.Products;

[Serializable]
public sealed class ProductVariant
{
    public int VariantId_VariantsUnion { get; set; }
    public int VariantId { get; set; }
    public int ProductId { get; set; }
    public string? VariantName { get; set; }
    [Column( TypeName = "decimal(18,2)" )] public decimal VariantPriceMain { get; set; }
    [Column( TypeName = "decimal(18,2)" )] public decimal VariantPriceSale { get; set; }
}