namespace BlazorElectronics.Shared.DataTransferObjects.Products;

public sealed class ProductReview_DTO
{
    public int User { get; set; }
    public int Rating { get; set; }
    public string Review { get; set; } = string.Empty;
}