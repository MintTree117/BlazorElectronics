namespace BlazorElectronics.Shared.DataTransferObjects.Products;

public sealed class ProductSearch_DTO
{
    public List<Product_DTO> Products { get; set; } = new List<Product_DTO>();
    public int NumPages { get; set; }
    public int CurrentPage { get; set; }
    public int NumResults { get; set; }
}