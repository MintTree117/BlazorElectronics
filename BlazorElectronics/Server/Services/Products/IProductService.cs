using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Server.Services.Products;

public interface IProductService
{
    Task<ServiceResponse<ProductList_DTO?>> GetProducts();
    Task<ServiceResponse<ProductDetails_DTO?>> GetProductDetails( int productId );
}