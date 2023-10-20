namespace BlazorElectronics.Shared.DtosOutbound.Products;

public sealed class ProductSearchResults_DTO
{
    public List<Product_DTO> Products { get; set; } = new();
    public int TotalMatches { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int ItemsPerPage { get; set; }
}