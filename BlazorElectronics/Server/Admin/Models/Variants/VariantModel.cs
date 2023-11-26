namespace BlazorElectronics.Server.Admin.Models.Variants;

public sealed class VariantModel
{
    public int VariantValueId { get; set; }
    public decimal VariantPrice { get; set; }
    public decimal? VariantSalePrice { get; set; }
}