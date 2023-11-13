using BlazorElectronics.Shared.Mutual;

namespace BlazorElectronics.Shared.Inbound.Products;

public sealed class ProductSuggestionRequest
{
    public ProductSuggestionRequest( string searchText, CategoryIdMap? categoryIdMap )
    {
        SearchText = searchText;
        CategoryIdMap = categoryIdMap;
    }
    
    public string SearchText { get; }
    public CategoryIdMap? CategoryIdMap { get; }
}