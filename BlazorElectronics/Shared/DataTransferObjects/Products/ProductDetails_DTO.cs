namespace BlazorElectronics.Shared.DataTransferObjects.Products;

public sealed class ProductDetails_DTO
{
    //public Product_DTO? Product { get; set; }
    //public string? Description { get; set; }
    //public List<ProductSpec_DTO>? Specs { get; set; }
    //public List<ProductImage_DTO>? Images { get; set; }
    //public List<ProductReview_DTO>? Reviews { get; set; }

    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductDescription { get; set; } = string.Empty;
    public List<ProductVariant_DTO>? ProductVariants { get; set; }
    public List<ProductImage_DTO>? ProductImages { get; set; }
    public List<ProductReview_DTO>? ProductReviews { get; set; }
}