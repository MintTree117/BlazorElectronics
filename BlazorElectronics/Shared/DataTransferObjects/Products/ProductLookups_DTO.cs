namespace BlazorElectronics.Shared.DataTransferObjects.Products;

public sealed class ProductLookups_DTO
{
    public Dictionary<string, Dictionary<int, Dictionary<string, object>>> Tables { get; set; } = new();
}