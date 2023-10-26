using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DtosInbound.Products;
using BlazorElectronics.Shared.DtosOutbound.Products;
using BlazorElectronics.Shared.Outbound.Features;

namespace BlazorElectronics.Client.Services.Products;

public interface IProductServiceClient
{
    event Action<string>? ExceptionEvent;
    event Action<ServiceResponse<ProductSearchResults_DTO?>?>? ProductSearchChanged;
    event Action<string>? ProductSearchNullabillityTest;
    
    ProductSearchRequest_DTO? SearchRequest { get; set; }

    void ClearSearchRequest();
    void UpdateSearchCategory( string url );
    void UpdateSearchText( string text );
    void UpdateSearchPage( int page );
    void UpdateSearchResultsCount( int count );
    string? GetProductSearchUrl();
    
    Task SearchProductsByCategory( string categoryUrl, ProductSearchRequest_DTO? filters );
    Task SearchProductsByText( string searchText, ProductSearchRequest_DTO? filters );
    Task SearchProductsByCategoryAndText( string categoryUrl, string searchText, ProductSearchRequest_DTO? filters );
    Task<ServiceResponse<ProductSearchSuggestions_DTO?>> GetProductSearchSuggestions( string searchText );
    Task<ServiceResponse<ProductDetails_DTO?>> GetProductDetails( int productId );
}