using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Server.Services.Products;

public interface IProductService
{
    Task<DtoResponse<string?>> TestGetQueryString( string categoryUrl, ProductSearchFilters_DTO filters );
    Task<DtoResponse<Products_DTO?>> GetProducts();
    Task<DtoResponse<ProductSearch_DTO?>> SearchProducts( string categoryUrl, ProductSearchFilters_DTO? searchFilters );
    Task<DtoResponse<ProductDetails_DTO?>> GetProductDetails( int productId );
}