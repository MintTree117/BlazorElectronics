using BlazorElectronics.Server.Models.Features;
using BlazorElectronics.Server.Models.Products;

namespace BlazorElectronics.Server.Repositories.Products;

public interface IProductSalesRepository
{
    Task<IEnumerable<ProductSale>?> GetProductSalesFeatured();
    Task<IEnumerable<ProductSale>?> GetProductSales();
}