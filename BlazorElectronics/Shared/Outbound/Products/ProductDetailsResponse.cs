namespace BlazorElectronics.Shared.DtosOutbound.Products;

[Serializable]
public class ProductDetailsResponse
{   
    public int Id { get; set; }
    public int Rating { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<ProductVariant_DTO> Variants { get; set; } = new();
    public List<ProductImage_DTO> Images { get; set; } = new();
    public List<ProductReview_DTO> Reviews { get; set; } = new();
}