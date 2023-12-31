namespace BlazorElectronics.Shared.Features;

public class FeatureDto
{
    public int FeatureId { get; protected set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
}