namespace BlazorElectronics.Server.Models.Products;

public sealed class ProductDescription
{
    public int DescriptionId { get; set; }
    public int ProductId { get; set; }
    public string? Description { get; set; }
}