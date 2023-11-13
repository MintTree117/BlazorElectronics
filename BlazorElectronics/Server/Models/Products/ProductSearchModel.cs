namespace BlazorElectronics.Server.Models.Products;

public sealed class ProductSearchModel
{
    public ProductSearchModel( int totalCount, int productId, string productTitle, int productRating, string productThumbnail )
    {
        TotalCount = totalCount;
        ProductId = productId;
        ProductTitle = productTitle;
        ProductRating = productRating;
        ProductThumbnail = productThumbnail;
    }
    
    public int TotalCount { get; }
    public int ProductId { get; }
    public string ProductTitle { get; }
    public int ProductRating { get; }
    public string ProductThumbnail { get; }
}