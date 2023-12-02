namespace BlazorElectronics.Shared.Products.Search;

public sealed class ProductSuggestionRequest
{
    public ProductSuggestionRequest( string searchText )
    {
        SearchText = searchText;
    }
    
    public string SearchText { get; }
}