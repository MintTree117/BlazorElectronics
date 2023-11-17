using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Server.Models.Products.Details;

namespace BlazorElectronics.Server.Repositories.Products;

public interface IProductDetailsRepository
{
    Task<ProductDetailsModel?> GetProductDetails( int productId );
}