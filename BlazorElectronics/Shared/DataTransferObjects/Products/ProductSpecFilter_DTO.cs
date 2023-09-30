namespace BlazorElectronics.Shared.DataTransferObjects.Products;

public sealed class ProductSpecFilter_DTO
{
    public int ValueId { get; set; }
    public int FilterType { get; set; }
    public string? SpecName { get; set; }
    public object? SpecValue { get; set; }
}

// STATIC
// valueid
// filtertype
// specname
// specvalue

// DYNAMIC
// 