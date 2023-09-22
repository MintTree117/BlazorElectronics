using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Server.Repositories.Products;

public interface IProductRepository
{
    Task<IEnumerable<Product>?> GetProducts();

    Task<IEnumerable<Product>> GetAllProducts();
    Task<IEnumerable<Product>> SearchProducts( ProductSearchFilters_DTO searchFilters );

    Task<ProductDetails> GetProductDetails( int productId );
}