namespace BlazorElectronics.Server.Models.Products;

public sealed class ProductDetails
{
    public Product Product { get; set; } = new();
    public ProductDescription? ProductDescription;
    public List<ProductImage> ProductImages { get; set; } = new();
    public List<ProductReview> ProductReviews { get; set; } = new();
}