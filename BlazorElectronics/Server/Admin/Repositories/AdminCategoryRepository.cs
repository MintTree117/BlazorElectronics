using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Repositories;
using BlazorElectronics.Shared.Admin.Categories;

namespace BlazorElectronics.Server.Admin.Repositories;

public class AdminCategoryRepository : DapperRepository, IAdminCategoryRepository
{
    public AdminCategoryRepository( DapperContext dapperContext )
        : base( dapperContext ) { }

    public Task<bool> AddCategory( AddCategoryDto dto )
    {
        throw new NotImplementedException();
    }
    public Task<bool> UpdateCategory( UpdateCategoryDto dto )
    {
        throw new NotImplementedException();
    }
    public Task<bool> DeleteCategory( DeleteCategoryDto dto )
    {
        throw new NotImplementedException();
    }
}