using BlazorElectronics.Server.Models.Products.Seed;

namespace BlazorElectronics.Server.Repositories.Products;

public interface IProductSeedRepository
{
    Task<bool> SeedProducts( IEnumerable<ProductSeedModel> models );
}