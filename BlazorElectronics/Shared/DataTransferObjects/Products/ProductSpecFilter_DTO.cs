namespace BlazorElectronics.Shared.DataTransferObjects.Products;

public sealed class ProductSpecFilter_DTO
{
    public int SpecType { get; set; }
    public int FilterType { get; set; }
    public int DataType { get; set; }
    public string? SpecName { get; set; }
    public object? SpecValue { get; set; }
}