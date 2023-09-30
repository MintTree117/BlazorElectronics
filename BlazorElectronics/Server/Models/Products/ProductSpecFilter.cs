using BlazorElectronics.Server.Models.Specs;

namespace BlazorElectronics.Server.Models.Products;

public sealed class ProductSpecFilter
{
    public string SpecName { get; set; }
    public int SpecId { get; set; }
    public object SpecValue { get; set; } = null!;
}