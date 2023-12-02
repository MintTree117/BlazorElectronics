using BlazorElectronics.Shared.Features;

namespace BlazorElectronics.Server.Models.Features;

public class FeaturesModel
{
    public IEnumerable<Feature>? Features { get; set; }
    public IEnumerable<FeaturedDeal>? Deals { get; set; }
}