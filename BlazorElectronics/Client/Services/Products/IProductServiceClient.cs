using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DtosOutbound.Products;
using BlazorElectronics.Shared.Inbound.Products;

namespace BlazorElectronics.Client.Services.Products;

public interface IProductServiceClient
{
    event Action<string>? ExceptionEvent;
    event Action<Reply<ProductSearchResults_DTO?>?>? ProductSearchChanged;
    event Action<string>? ProductSearchNullabillityTest;
    
    ProductSearchRequest? SearchRequest { get; set; }

    void ClearSearchRequest();
    void UpdateSearchCategory( string url );
    void UpdateSearchText( string text );
    void UpdateSearchPage( int page );
    void UpdateSearchResultsCount( int count );
    string? GetProductSearchUrl();
    
    Task SearchProductsByCategory( ProductSearchRequest filters, string primary, string? secondary = null, string? tertiary = null );
    Task<Reply<ProductSearchSuggestions_DTO?>> GetProductSearchSuggestions( string searchText );
    Task<Reply<ProductDetails_DTO?>> GetProductDetails( int productId );
}