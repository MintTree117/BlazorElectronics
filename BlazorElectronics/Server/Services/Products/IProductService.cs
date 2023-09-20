using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Server.Services.Products;

public interface IProductService
{
    Task<ServiceResponse<List<Product_DTO>?>> GetProducts();
    Task<ServiceResponse<ProductDetails_DTO?>> GetProductDetails();
}