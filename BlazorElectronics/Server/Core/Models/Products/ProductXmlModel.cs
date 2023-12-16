namespace BlazorElectronics.Server.Core.Models.Products;

public sealed class ProductXmlModel
{
    public int ProductId { get; set; }
    public string XmlSpecs { get; set; } = string.Empty;
}