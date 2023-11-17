using BlazorElectronics.Shared.Outbound.Products;

namespace BlazorElectronics.Shared.DtosOutbound.Products;

public sealed class ProductResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
    public int Rating { get; set; }
    public List<ProductVariantResponse> Variants { get; set; } = new();
}