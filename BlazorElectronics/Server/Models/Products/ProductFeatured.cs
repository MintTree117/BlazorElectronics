namespace BlazorElectronics.Server.Models.Products;

public sealed class ProductFeatured
{
    public int FeatureId { get; set; }
    public int ProductId { get; set; }
    public string? FeatureImageUrl { get; set; }
}