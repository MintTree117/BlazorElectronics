namespace BlazorElectronics.Shared.Admin.Products;

public sealed class AddUpdateProductVariantDto
{
    public AddUpdateProductVariantDto( int variantId, decimal originalPrice, decimal salePrice )
    {
        VariantId = variantId;
        OriginalPrice = originalPrice;
        SalePrice = salePrice;
    }
    
    public int VariantId { get; }
    public decimal OriginalPrice  { get; }
    public decimal SalePrice { get; }
}