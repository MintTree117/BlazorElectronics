namespace BlazorElectronics.Shared.Features;

public sealed class FeaturesResponse
{
    public FeaturesResponse()
    {
        
    }
    public FeaturesResponse( List<Feature> features, List<FeaturedDeal> deals )
    {
        Features = features;
        Deals = deals;
    }
    
    public List<Feature> Features { get; init; } = new();
    public List<FeaturedDeal> Deals { get; init; } = new();
}