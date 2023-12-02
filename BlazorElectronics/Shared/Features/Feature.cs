namespace BlazorElectronics.Shared.Features;

public sealed class Feature
{
    public int FeatureId { get; set; }
    public string FeatureName { get; set; } = string.Empty;
    public string FeatureUrl { get; set; } = string.Empty;
    public string FeatureImage { get; set; } = string.Empty;
}