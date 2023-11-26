using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DtosOutbound.Products;
using BlazorElectronics.Shared.Outbound.Products;
using BlazorElectronics.Shared.Products;

namespace BlazorElectronics.Client.Services.Products;

public interface IProductServiceClient
{
    event Action<string>? ExceptionEvent;
    event Action<ApiReply<ProductSearchResponse?>?>? ProductSearchChanged;
    event Action<string>? ProductSearchNullabillityTest;
    
    ProductSearchRequest? SearchRequest { get; set; }

    void ClearSearchRequest();
    void UpdateSearchCategory( string url );
    void UpdateSearchText( string text );
    void UpdateSearchPage( int page );
    void UpdateSearchResultsCount( int count );
    string? GetProductSearchUrl();
    
    Task SearchProductsByCategory( ProductSearchRequest filters, string primary, string? secondary = null, string? tertiary = null );
    Task<ApiReply<ProductSuggestionsResponse?>> GetProductSearchSuggestions( string searchText );
    Task<ApiReply<ProductDetailsResponse?>> GetProductDetails( int productId );
}