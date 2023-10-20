using BlazorElectronics.Server.Models.Products;

namespace BlazorElectronics.Server.Repositories.Products;

public interface IProductCartRepository : IDapperRepository<Product>
{
    Task<IEnumerable<Product>?> GetCartItems( List<int> productIds, List<int> variantIds );
}