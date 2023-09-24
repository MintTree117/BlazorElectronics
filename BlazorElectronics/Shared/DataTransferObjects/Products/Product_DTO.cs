namespace BlazorElectronics.Shared.DataTransferObjects.Products;

public sealed class Product_DTO
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
    public int Rating { get; set; }

    public List<ProductVariant_DTO> Variants { get; set; } = new();
}