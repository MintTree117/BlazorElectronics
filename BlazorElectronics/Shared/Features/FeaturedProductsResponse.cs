namespace BlazorElectronics.Shared.Features;

public sealed class FeaturedProductsResponse
{
    public List<FeaturedProduct_DTO> FeaturedProducts { get; set; } = new();
}