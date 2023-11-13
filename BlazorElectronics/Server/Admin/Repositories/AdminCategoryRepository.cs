using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Repositories;
using BlazorElectronics.Shared.Admin.Categories;

namespace BlazorElectronics.Server.Admin.Repositories;

public class AdminCategoryRepository : DapperRepository, IAdminCategoryRepository
{
    static readonly string[] PROCEDURES_ADD_CATEGORY = { "Add_CategoryPrimary", "Add_CategorySecondary", "Add_CategoryTertiary" };
    static readonly string[] PROCEDURES_UPDATE_CATEGORY = { "Update_CategoryPrimary", "Update_CategorySecondary", "Update_CategoryTertiary" };
    static readonly string[] PROCEDURES_DELETE_CATEGORY = { "Remove_CategoryPrimary", "Remove_CategorySecondary", "Remove_CategoryTertiary" };
    
    
    
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