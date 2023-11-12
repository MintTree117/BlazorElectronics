using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Repositories;
using BlazorElectronics.Shared.Admin.Products;

namespace BlazorElectronics.Server.Admin.Repositories;

public class AdminProductRepository : DapperRepository, IAdminProductRepository
{
    public AdminProductRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
    
    public Task<bool> AddProduct( AddUpdateProductDto dto )
    {
        throw new NotImplementedException();
    }
    public Task<bool> UpdateProduct( AddUpdateProductDto dto )
    {
        throw new NotImplementedException();
    }
    public Task<bool> RemoveProduct( RemoveProductDto dto )
    {
        throw new NotImplementedException();
    }
}