using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorElectronics.Shared.DtosOutbound.Products;

public sealed class ProductVariant_DTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    [Column( TypeName = "decimal(18,2)" )] public decimal Price { get; set; }
    [Column( TypeName = "decimal(18,2)" )] public decimal SalePrice { get; set; }
}