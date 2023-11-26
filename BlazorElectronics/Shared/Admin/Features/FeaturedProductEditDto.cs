namespace BlazorElectronics.Shared.Admin.Features;

public sealed class FeaturedProductEditDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}