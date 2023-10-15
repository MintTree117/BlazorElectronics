namespace BlazorElectronics.Shared.DtosOutbound.Products;

public sealed class ProductSearchResults_DTO
{
    public List<Product_DTO> Products { get; set; } = new();
    public int Pages { get; set; }
    public int CurrentPage { get; set; }
    public int TotalMatches { get; set; }

    public int Offset { get; set; }
    public int Rows { get; set; }
}