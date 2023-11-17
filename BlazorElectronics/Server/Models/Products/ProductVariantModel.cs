using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorElectronics.Server.Models.Products;

public sealed class ProductVariantModel
{
    public ProductVariantModel( int productId, int variantId, string variantName, decimal variantPriceMain, decimal? variantPriceSale )
    {
        ProductId = productId;
        VariantId = variantId;
        VariantName = variantName;
        VariantPriceMain = variantPriceMain;
        VariantPriceSale = variantPriceSale;
    }
    
    public int ProductId { get; }
    public int VariantId { get; }
    public string VariantName { get; }
    [Column( TypeName = "decimal(18,2)" )] public decimal VariantPriceMain { get; }
    [Column( TypeName = "decimal(18,2)" )] public decimal? VariantPriceSale { get; }
}