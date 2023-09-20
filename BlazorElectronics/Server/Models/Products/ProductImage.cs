namespace BlazorElectronics.Server.Models.Products;

public sealed class ProductImage
{
    public int ImageId { get; set; }
    public int ProductId { get; set; }
    public int ProductVariantId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}