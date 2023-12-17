using BlazorElectronics.Shared.Enums;

namespace BlazorElectronics.Shared.Products.Search;

public class ProductSearchRequestDto
{
    public int Page { get; set; } = 1;
    public int Rows { get; set; } = 10;
    public int? CategoryId { get; set; }
    public string? SearchText { get; set; }
    public ProductSortType SortType { get; set; }
    public ProductFiltersDto? Filters { get; set; }
}