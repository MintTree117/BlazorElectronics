using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Server.Services.Products;

public interface IProductService
{
    Task<ServiceResponse<string>> TestGetQueryString( ProductSearchFilters_DTO filters );
    Task<ServiceResponse<Products_DTO?>> GetProducts( ProductSearchFilters_DTO? searchFilters = null );
    Task<ServiceResponse<ProductDetails_DTO?>> GetProductDetails( int productId );
}