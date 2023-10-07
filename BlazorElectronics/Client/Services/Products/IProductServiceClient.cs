using BlazorElectronics.Shared;
using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Client.Services.Products;

public interface IProductServiceClient
{
    event Action<ServiceResponse<ProductSearch_DTO?>> ProductSearchChanged;
    
    Task SearchProducts( ProductSearchFilters_DTO? filters );
    Task<ServiceResponse<ProductDetails_DTO?>> GetProductDetails( int productId );
}