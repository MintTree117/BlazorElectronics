namespace BlazorElectronics.Shared.Features;

public sealed class FeaturesReplyDto
{
    public FeaturesReplyDto()
    {
        
    }
    public FeaturesReplyDto( List<FeatureDto> features, List<FeaturedDealDto> deals )
    {
        Features = features;
        Deals = deals;
    }
    
    public List<FeatureDto> Features { get; init; } = new();
    public List<FeaturedDealDto> Deals { get; init; } = new();
}