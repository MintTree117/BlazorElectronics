namespace BlazorElectronics.Shared.Features;

public class FeaturesResponse
{
    public List<FeaturedProductDto> FeaturedProducts { get; set; } = new();
    public List<FeaturedDealDto> FeaturedDeals { get; set; } = new();
}