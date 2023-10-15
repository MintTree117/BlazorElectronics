namespace BlazorElectronics.Server.Models.Products;

[Serializable]
public sealed class Product
{
    public int ProductId { get; set; }
    public string ProductTitle { get; set; } = string.Empty;
    public int ProductRating { get; set; }
    public string ProductThumbnail { get; set; } = string.Empty;
    public List<ProductVariant> ProductVariants { get; set; } = new();
}