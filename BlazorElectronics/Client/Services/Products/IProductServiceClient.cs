using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Client.Services.Products;

public interface IProductServiceClient
{
    event Action<ServiceResponse<ProductSearch_DTO?>?>? ProductSearchChanged;

    Task SearchProductsByCategory( string categoryUrl, ProductSearchFilters_DTO? filters );
    Task SearchProductsByText( string searchText, ProductSearchFilters_DTO? filters );
    Task SearchProductsByCategoryAndText( string categoryUrl, string searchText, ProductSearchFilters_DTO? filters );
    Task<ServiceResponse<ProductSearchSuggestions_DTO?>> GetProductSearchSuggestions( string searchText );
    Task<ServiceResponse<ProductDetails_DTO?>> GetProductDetails( int productId );
}