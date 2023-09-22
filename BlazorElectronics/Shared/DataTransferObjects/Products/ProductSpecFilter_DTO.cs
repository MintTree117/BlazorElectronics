namespace BlazorElectronics.Shared.DataTransferObjects.Products;

public sealed class ProductSpecFilter_DTO
{
    public bool IsRaw { get; set; }
    public string? FilterName { get; set; }
    public object? FilterValue { get; set; }
}