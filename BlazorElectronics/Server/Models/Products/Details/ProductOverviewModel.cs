namespace BlazorElectronics.Server.Models.Products.Details;

public sealed class ProductOverviewModel
{
    public int VariantTypeId { get; init; }
    public string Title { get; init; } = string.Empty;
    public decimal Rating { get; init; }
    public DateTime ReleaseDate { get; init; }
    public bool HasDrm { get; init; }
    public int NumberSold { get; init; }
}