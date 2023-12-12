namespace BlazorElectronics.Server.Core.Models.Products;

public sealed class ProductReview
{
    public int ReviewId { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public int Rating { get; set; }
    public string Review { get; set; } = string.Empty;
}