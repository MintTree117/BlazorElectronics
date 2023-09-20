namespace BlazorElectronics.Server.Models.Products;

public sealed class ProductDescription
{
    public int DescriptionId { get; set; }
    public string? DescriptionBody { get; set; }
    public int FK_ProductDescription_ProductId { get; set; }
}