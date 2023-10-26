using BlazorElectronics.Server.Models.Products;

namespace BlazorElectronics.Server.Repositories.Products;

public interface IProductDetailsRepository
{
    Task<ProductDetails?> GetProductDetailsById( int id );
}