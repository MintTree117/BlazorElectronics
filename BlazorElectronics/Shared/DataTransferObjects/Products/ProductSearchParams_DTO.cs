namespace BlazorElectronics.Shared.DataTransferObjects.Products;

public sealed class ProductSearchParams_DTO
{
    public int Page { get; set; } = 0;
    public int Rows { get; set; } = 1;

    public string? SearchText { get; set; }
    public string? Category { get; set; }

    public int? MinPrice { get; set; } = 0;
    public int? MaxPrice { get; set; }
    public int? MinRating { get; set; }

    public List<ProductSpecFilter_DTO>? SpecFilters { get; set; }
}