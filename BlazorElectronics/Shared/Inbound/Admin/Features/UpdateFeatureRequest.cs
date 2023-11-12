namespace BlazorElectronics.Shared.Inbound.Admin.Features;

public sealed class UpdateFeatureRequest : AddOrUpdateFeatureRequest
{
    public int FeatureId { get; set; }
}