using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Server.Services.Products;

public interface IProductService
{
    Task<DtoResponse<string?>> TestGetQueryString( ProductSearchFilters_DTO filters );
    Task<DtoResponse<Products_DTO?>> GetProducts( ProductSearchFilters_DTO? searchFilters = null );
    Task<DtoResponse<ProductDetails_DTO?>> GetProductDetails( int productId );
}