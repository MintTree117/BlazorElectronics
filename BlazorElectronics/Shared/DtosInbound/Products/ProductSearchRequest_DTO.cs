namespace BlazorElectronics.Shared.DtosInbound.Products;

public sealed class ProductSearchRequest_DTO
{
    public int Page { get; set; } = 0;
    public int NumberOfResults { get; set; } = 5;

    public string? CategoryUrl { get; set; }
    public string? SearchText { get; set; }

    public int? MinPrice { get; set; } = null;
    public int? MaxPrice { get; set; } = null;
    public int? MinRating { get; set; } = null;
    public int? MaxRating { get; set; } = null;

    public List<ProductSpecFilter_DTO>? SpecFilters { get; set; }
}