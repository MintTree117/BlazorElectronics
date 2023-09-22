using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Server.Services.Products;

public interface IProductService
{
    Task<ServiceResponse<ProductList_DTO?>> GetProducts( ProductSearchFilters_DTO? searchFilters = null );
    Task<ServiceResponse<ProductDetails_DTO?>> GetProductDetails( int productId );
}