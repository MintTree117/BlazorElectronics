using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Categories;
using BlazorElectronics.Shared.Categories;
using Dapper;

namespace BlazorElectronics.Server.Repositories.Categories;

public class CategoryRepository : DapperRepository, ICategoryRepository
{
    const string PROCEDURE_GET = "Get_Categories";
    const string PROCEDURE_GET_VIEW = "Get_CategoriesView";
    const string PROCEDURE_GET_EDIT = "Get_CategoryEdit";
    const string PROCEDURE_INSERT = "Insert_Category";
    const string PROCEDURE_UPDATE = "Update_Category";
    const string PROCEDURE_DELETE = "Delete_Category";

    public CategoryRepository( DapperContext dapperContext )
        : base( dapperContext ) { }

    public async Task<IEnumerable<CategoryModel>?> Get()
    {
        return await TryQueryAsync( Query<CategoryModel>, null, PROCEDURE_GET );
    }
    public async Task<CategoryModel?> GetEdit( int categoryId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_CATEGORY_ID, categoryId );
        return await TryQueryAsync( QuerySingleOrDefault<CategoryModel?>, p, PROCEDURE_GET_EDIT );
    }
    public async Task<int> Insert( CategoryEditDto dto )
    {
        DynamicParameters p = GetAddParameters( dto );
        return await TryQueryTransactionAsync( QuerySingleOrDefaultTransaction<int>, p, PROCEDURE_INSERT );
    }
    public async Task<bool> Update( CategoryEditDto dto )
    {
        DynamicParameters p = GetUpdateParameters( dto );
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_UPDATE );
    }
    public async Task<bool> Delete( int categoryId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_CATEGORY_ID, categoryId );
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_DELETE );
    }
    
    static DynamicParameters GetAddParameters( CategoryEditDto dto )
    {
        DynamicParameters parameters = GetUpdateParameters( dto );
        parameters.Add( PARAM_CATEGORY_ID, dto.CategoryId );
        return parameters;
    }
    static DynamicParameters GetUpdateParameters( CategoryEditDto dto )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_CATEGORY_NAME, dto.Name );
        parameters.Add( PARAM_CATEGORY_API_URL, dto.ApiUrl );
        parameters.Add( PARAM_CATEGORY_IMAGE_URL, dto.ImageUrl );
        return parameters;
    }
}