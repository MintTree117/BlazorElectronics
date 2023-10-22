using BlazorElectronics.Server.Models.Products;

namespace BlazorElectronics.Server.Repositories.Cart;

public interface ICartRepository : IDapperRepository<Product>
{
    Task<IEnumerable<Product>?> GetCartItems( List<int> productIds, List<int> variantIds );
}