using BlazorElectronics.Shared.DtosOutbound.Products;
using BlazorElectronics.Shared.Products.Search;

namespace BlazorElectronics.Shared.Products;

public sealed class Products_DTO
{
    public List<ProductResponse> Products { get; set; } = new();
}