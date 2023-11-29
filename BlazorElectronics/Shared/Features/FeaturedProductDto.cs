namespace BlazorElectronics.Shared.Features;

public sealed class FeaturedProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}