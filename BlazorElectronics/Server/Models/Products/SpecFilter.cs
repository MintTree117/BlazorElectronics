namespace BlazorElectronics.Server.Models.Products;

public sealed class SpecFilter
{
    public string SpecName { get; set; } = string.Empty;
    public object SpecValue { get; set; } = null!;
}