namespace BlazorElectronics.Shared.DataTransferObjects.Products;

public sealed class ProductCategory_DTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}