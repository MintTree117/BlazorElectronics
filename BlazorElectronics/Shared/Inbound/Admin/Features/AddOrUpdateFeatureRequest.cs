namespace BlazorElectronics.Shared.Inbound.Admin.Features;

public abstract class AddOrUpdateFeatureRequest : AdminRequest
{
    public string? FeatureImageUrl { get; set; }
    public string? FeaturePageUrl { get; set; }
}