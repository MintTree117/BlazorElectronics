namespace BlazorElectronics.Shared.Features;

public sealed class Feature
{
    public int FeatureId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
}