namespace BlazorElectronics.Server.Models.Products;

public sealed class ValidatedSearchFilters
{
    public int Page { get; set; } = 0;
    public int Rows { get; set; } = 1;

    public string? SearchText { get; set; }
    public int? Category { get; set; }

    public int? MinPrice { get; set; }
    public int? MaxPrice { get; set; }
    public int? MinRating { get; set; }
    public int? MaxRating { get; set; }

    public List<SpecFilter> LookupSpecFilters { get; set; } = new();
    public List<SpecFilter> RawSpecFilters { get; set; } = new();
}