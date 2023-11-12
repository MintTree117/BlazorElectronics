namespace BlazorElectronics.Shared.Outbound.Features;

public sealed class FeaturedProductsResponse
{
    public List<FeaturedProduct_DTO> FeaturedProducts { get; set; } = new();
}