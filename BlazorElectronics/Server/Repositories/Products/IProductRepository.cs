using BlazorElectronics.Server.Models.Products;

namespace BlazorElectronics.Server.Repositories.Products;

public interface IProductRepository
{
    Task<ProductModel?> Get( int productId );
    Task<int> Insert( ProductModel model );
    Task<bool> Update( ProductModel model );
    Task<bool> Delete( int productId );
}