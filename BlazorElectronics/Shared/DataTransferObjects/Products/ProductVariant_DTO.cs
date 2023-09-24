using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorElectronics.Shared.DataTransferObjects.Products;

public sealed class ProductVariant_DTO
{
    public int VariantId { get; set; }
    public string VariantName { get; set; } = string.Empty;
    [Column( TypeName = "decimal(18,2)" )] public decimal Price { get; set; }
    [Column( TypeName = "decimal(18,2)" )] public decimal SalePrice { get; set; }
}