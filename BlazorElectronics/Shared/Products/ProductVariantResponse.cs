using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorElectronics.Shared.Products;

public sealed class ProductVariantResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    [Column( TypeName = "decimal(18,2)" )] public decimal Price { get; init; }
    [Column( TypeName = "decimal(18,2)" )] public decimal? SalePrice { get; init; }
}