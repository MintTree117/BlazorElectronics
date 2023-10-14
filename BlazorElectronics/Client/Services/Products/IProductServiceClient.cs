using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Client.Services.Products;

public interface IProductServiceClient
{
    event Action<ServiceResponse<ProductSearchResults_DTO?>?>? ProductSearchChanged;
    
    ProductSearchRequest_DTO? SearchRequest { get; set; }

    void UpdateProductSearchPage( int page );
    void UpdateProductSearchResultsCount( int count );
    string GetProductSearchUrl();

    Task<ServiceResponse<ProductsFeatured_DTO?>?> GetFeaturedProducts();
    Task SearchProductsByCategory( string categoryUrl, ProductSearchRequest_DTO? filters );
    Task SearchProductsByText( string searchText, ProductSearchRequest_DTO? filters );
    Task SearchProductsByCategoryAndText( string categoryUrl, string searchText, ProductSearchRequest_DTO? filters );
    Task<ServiceResponse<ProductSearchSuggestions_DTO?>?> GetProductSearchSuggestions( string searchText );
    Task<ServiceResponse<ProductDetails_DTO?>> GetProductDetails( int productId );
}