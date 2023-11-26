namespace BlazorElectronics.Shared.Admin.Features;

public class FeaturesViewDto
{
    public List<FeaturedProductEditDto> FeaturedProducts { get; set; } = new();
    public List<FeaturedDealEditDto> FeaturedDeals { get; set; } = new();
}