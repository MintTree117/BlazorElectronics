namespace BlazorElectronics.Shared.Admin.Products;

public sealed class AddUpdateProductImagesDto
{
    public AddUpdateProductImagesDto( int variantIndex, string imageUrl )
    {
        VariantIndex = variantIndex;
        ImageUrl = imageUrl;
    }
    
    public int VariantIndex { get; }
    public string ImageUrl { get; }
}