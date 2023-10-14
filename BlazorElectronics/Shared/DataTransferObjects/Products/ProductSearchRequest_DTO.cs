namespace BlazorElectronics.Shared.DataTransferObjects.Products;

public sealed class ProductSearchRequest_DTO
{
    public int Page { get; set; } = 0;
    public int Rows { get; set; } = 5;

    public string? CategoryUrl { get; set; } = null;
    public string? SearchText { get; set; } = null;

    public int? MinPrice { get; set; } = null;
    public int? MaxPrice { get; set; } = null;
    public int? MinRating { get; set; } = null;
    public int? MaxRating { get; set; } = null;

    public List<ProductSpecFilter_DTO>? SpecFilters { get; set; } = new();
}