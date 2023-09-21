using BlazorElectronics.Server.Models.Products;

namespace BlazorElectronics.Server.Repositories.Products;

public interface IProductRepository
{
    Task<IEnumerable<Product>?> GetProducts();
    Task<ProductDetails> GetProductDetails( int productId );
}