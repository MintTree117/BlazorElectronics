namespace BlazorElectronics.Server.Core.Models.Products;

public sealed class ProductDescriptionModel
{
    public int ProductId { get; set; }
    public string Description { get; set; } = string.Empty;
}