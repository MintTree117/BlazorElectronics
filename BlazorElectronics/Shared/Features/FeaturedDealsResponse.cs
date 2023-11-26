namespace BlazorElectronics.Shared.Features;

public sealed class FeaturedDealsResponse
{
    public List<FeaturedDeal_DTO> Deals { get; set; } = new();
}