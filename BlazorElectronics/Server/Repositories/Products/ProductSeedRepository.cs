using BlazorElectronics.Server.DbContext;

namespace BlazorElectronics.Server.Repositories.Products;

public class ProductSeedRepository : DapperRepository, IProductSeedRepository
{
    public ProductSeedRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
}