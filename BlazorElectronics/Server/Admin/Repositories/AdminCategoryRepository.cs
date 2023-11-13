using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Repositories;
using BlazorElectronics.Shared.Admin.Categories;

namespace BlazorElectronics.Server.Admin.Repositories;

public class AdminCategoryRepository : DapperRepository, IAdminCategoryRepository
{
    static readonly string[] PROCEDURE_ADD_CATEGORY = { "", "", "" };
    static readonly string[] PROCEDURE_UPDATE_CATEGORY = { "", "", "" };
    static readonly string[] PROCEDURE_DELETE_CATEGORY = { "", "", "" };

    public AdminCategoryRepository( DapperContext dapperContext )
        : base( dapperContext ) { }

    public Task<bool> AddCategory( AddUpdateCategoryDto dto )
    {
        throw new NotImplementedException();
    }
    public Task<bool> UpdateCategory( AddUpdateCategoryDto dto )
    {
        throw new NotImplementedException();
    }
    public Task<bool> DeleteCategory( DeleteCategoryDto dto )
    {
        throw new NotImplementedException();
    }
}