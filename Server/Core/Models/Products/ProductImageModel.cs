namespace BlazorElectronics.Server.Core.Models.Products;

public sealed class ProductImageModel
{
    public int ProductId { get; set; }
    public string ImageUrl { get; init; } = string.Empty;
}