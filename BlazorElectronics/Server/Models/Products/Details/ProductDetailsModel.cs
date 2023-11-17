namespace BlazorElectronics.Server.Models.Products.Details;

public sealed class ProductDetailsModel
{
    public int PrimaryCategory { get; init; }
    public string? SecondaryCategories { get; init; } = string.Empty;
    public string? TertiaryCategories { get; init; } = string.Empty;
    public ProductOverviewModel? Overview { get; init; } = new();
    public string? ProductDescription { get; init; } = string.Empty;
    public List<ProductImageModel> Images { get; init; } = new();
    public List<ProductVariantModel> Variants { get; init; } = new();
}