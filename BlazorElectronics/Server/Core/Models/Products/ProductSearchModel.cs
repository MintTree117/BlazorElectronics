namespace BlazorElectronics.Server.Core.Models.Products;

public sealed class ProductSearchModel
{
    public int TotalCount { get; init; }
    public int ProductId { get; init; }
    public string ProductTitle { get; init; } = string.Empty;
    public float ProductRating { get; init; }
    public string ProductThumbnail { get; init; } = string.Empty;
}