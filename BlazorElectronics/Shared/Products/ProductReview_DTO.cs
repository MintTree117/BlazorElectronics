namespace BlazorElectronics.Shared.Products;

public sealed class ProductReview_DTO
{
    public int UserId { get; set; }
    public int Rating { get; set; }
    public string Review { get; set; } = string.Empty;
}