using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Server.Services.Products;

public interface IProductService
{
    Task<ServiceResponse<string?>> TestGetQueryString( string categoryUrl, ProductSearchFilters_DTO filters );
    Task<ServiceResponse<Products_DTO?>> GetProducts();
    Task<ServiceResponse<ProductSearchSuggestions_DTO?>> GetProductSearchSuggestions( string searchText );
    Task<ServiceResponse<ProductSearch_DTO?>> SearchProducts( ProductSearchFilters_DTO? searchFilters );
    Task<ServiceResponse<ProductDetails_DTO?>> GetProductDetails( int productId );
}