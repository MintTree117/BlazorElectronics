using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Shared.DataTransferObjects.Products;

namespace BlazorElectronics.Server.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>?> GetProducts();
    Task<ProductDetails> GetProductDetails();
}