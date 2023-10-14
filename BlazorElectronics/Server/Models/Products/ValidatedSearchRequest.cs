namespace BlazorElectronics.Server.Models.Products;

public sealed class ValidatedSearchRequest
{
    public int Page { get; set; } = 0;
    public int Rows { get; set; } = 10;

    public int? CategoryId { get; set; }
    public string? SearchText { get; set; }

    public int? MinPrice { get; set; }
    public int? MaxPrice { get; set; }
    public int? MinRating { get; set; }
    public int? MaxRating { get; set; }

    public List<ProductSpecFilter> LookupSpecFilters { get; set; } = new();
    public List<ProductSpecFilter> RawSpecFilters { get; set; } = new();
}