namespace BlazorElectronics.Shared.DataTransferObjects.Products;

public sealed class ProductSearchFilters_DTO
{
    public int Page { get; set; } = 0;
    public int Rows { get; set; } = 1;

    public string? SearchText { get; set; }

    public int MinPrice { get; set; } = -1;
    public int MaxPrice { get; set; } = -1;
    public int MinRating { get; set; } = -1;
    public int MaxRating { get; set; } = -1;

    public List<ProductSpecFilter_DTO> SpecFilters { get; set; } = new();
}