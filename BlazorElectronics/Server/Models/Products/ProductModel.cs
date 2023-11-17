namespace BlazorElectronics.Server.Models.Products;

public sealed class ProductModel
{
    public ProductModel( int productId, string productTitle, int productRating, string productThumbnail )
    {
        ProductId = productId;
        ProductTitle = productTitle;
        ProductRating = productRating;
        ProductThumbnail = productThumbnail;
    }
    
    public int ProductId { get; }
    public string ProductTitle { get; }
    public int ProductRating { get; }
    public string ProductThumbnail { get; }
}