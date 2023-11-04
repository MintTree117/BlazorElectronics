using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Shared.Inbound.Products;

public sealed class ProductSuggestionRequest
{
    public string? SearchText { get; set; }
    public CategoryIdMap? CategoryIdMap { get; set; }
}