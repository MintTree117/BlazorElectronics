namespace BlazorElectronics.Server.Models.Features;

public sealed class FeaturedProduct
{
    public int FeatureId { get; set; }
    public int ProductId { get; set; }
    public string? FeatureImageUrl { get; set; }
}