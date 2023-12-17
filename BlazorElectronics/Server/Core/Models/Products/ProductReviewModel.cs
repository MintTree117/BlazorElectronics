namespace BlazorElectronics.Server.Core.Models.Products;

public sealed class ProductReviewModel
{
    public int ReviewId { get; set; }
    public int ProductId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int Rating { get; set; }
    public string Review { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}