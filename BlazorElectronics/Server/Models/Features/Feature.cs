namespace BlazorElectronics.Server.Models.Features;

public sealed class Feature
{
    public int? ProductId { get; set; } = null;
    public string FeatureImageUrl { get; set; } = string.Empty;
    public string FeaturePageUrl { get; set; } = string.Empty;
}