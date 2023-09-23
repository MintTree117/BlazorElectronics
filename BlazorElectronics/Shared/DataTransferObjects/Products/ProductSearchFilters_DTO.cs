namespace BlazorElectronics.Shared.DataTransferObjects.Products;

public sealed class ProductSearchFilters_DTO
{
    public int Page { get; set; } = 0;
    public int Rows { get; set; } = 1;

    public string? SearchText { get; set; }
    public string? Category { get; set; }

    public int? MinPrice { get; set; }
    public int? MaxPrice { get; set; }
    public int? MinRating { get; set; }
    public int? MaxRating { get; set; }

    public List<ProductSpecFilter_DTO>? SpecFilters { get; set; }
}