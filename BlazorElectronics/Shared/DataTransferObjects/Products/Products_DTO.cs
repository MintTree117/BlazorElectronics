namespace BlazorElectronics.Shared.DataTransferObjects.Products;

public sealed class Products_DTO
{
    public int Page { get; set; }
    public int Count { get; set; }
    public List<Product_DTO> Products { get; set; } = new();
}