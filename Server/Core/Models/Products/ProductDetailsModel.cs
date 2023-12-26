namespace BlazorElectronics.Server.Core.Models.Products;

public sealed class ProductDetailsModel
{
    public ProductSummaryModel? Product { get; set; }
    public ProductDescriptionModel? Description { get; set; } = new();
    public IEnumerable<ProductCategoryModel> Categories { get; set; } = new List<ProductCategoryModel>();
    public IEnumerable<ProductImageModel> Images { get; set; } = new List<ProductImageModel>();
    public IEnumerable<ProductSpecLookupModel> SpecLookups { get; set; } = new List<ProductSpecLookupModel>();
    public ProductXmlModel? XmlSpecs { get; set; } = new();
}