namespace BlazorElectronics.Shared.DataTransferObjects.Products;

[Serializable]
public class ProductDetails_DTO
{   
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductDescription { get; set; } = string.Empty;
    public List<ProductVariant_DTO> ProductVariants { get; set; } = new();
    public List<ProductImage_DTO> ProductImages { get; set; } = new();
    public List<ProductReview_DTO> ProductReviews { get; set; } = new();
}