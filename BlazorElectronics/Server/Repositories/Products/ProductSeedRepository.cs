using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Products.Seed;

namespace BlazorElectronics.Server.Repositories.Products;

public class ProductSeedRepository : DapperRepository, IProductSeedRepository
{
    public ProductSeedRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
    
    public Task<bool> SeedProducts( IEnumerable<ProductSeedModel> models )
    {
        throw new NotImplementedException();
    }
}