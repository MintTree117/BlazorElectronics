namespace BlazorElectronics.Shared.Features;

public class Feature
{
    public int FeatureId { get; protected set; }
    public string Name { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string Image { get; init; } = string.Empty;
}