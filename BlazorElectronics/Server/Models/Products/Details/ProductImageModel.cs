namespace BlazorElectronics.Server.Models.Products.Details;

public sealed class ProductImageModel
{
    public ProductImageModel( int variantId, string imageUrl )
    {
        VariantId = variantId;
        ImageUrl = imageUrl;
    }
    
    public int VariantId { get; }
    public string ImageUrl { get; }
}