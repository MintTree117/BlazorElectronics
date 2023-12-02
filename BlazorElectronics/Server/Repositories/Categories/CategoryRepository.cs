using System.Data;
using System.Data.Common;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Categories;
using BlazorElectronics.Shared.Categories;
using BlazorElectronics.Shared.Enums;
using Dapper;
using Microsoft.Data.SqlClient;

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
        return await TryQueryAsync( GetCategoriesQuery );
    }
    public async Task<CategoryModel?> GetEdit( int categoryId )
    {
        DynamicParameters parameters = new();
        parameters.Add( PARAM_CATEGORY_ID, categoryId );
        
        return await TryQueryAsync( GetEditQuery, parameters );
    }
    public async Task<CategoryModel?> Insert( CategoryEditDto dto )
    {
        DynamicParameters parameters = GetAddParameters( dto );

        return await TryQueryTransactionAsync( InsertQuery, parameters );
    }
    public async Task<bool> Update( CategoryEditDto dto )
    {
        DynamicParameters parameters = GetUpdateParameters( dto );

        return await TryQueryTransactionAsync( UpdateQuery, parameters );
    }
    public async Task<bool> Delete( int categoryId )
    {
        DynamicParameters parameters = new();
        parameters.Add( PARAM_CATEGORY_ID, categoryId );

        return await TryQueryTransactionAsync( DeleteQuery, parameters );
    }

    static async Task<IEnumerable<CategoryModel>?> GetCategoriesQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QueryAsync<CategoryModel>( PROCEDURE_GET, commandType: CommandType.StoredProcedure );
    }
    static async Task<CategoryModel?> GetEditQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? parameters )
    {
        return await connection.QuerySingleOrDefaultAsync<CategoryModel>( PROCEDURE_GET_EDIT, parameters, commandType: CommandType.StoredProcedure );
    }
    static async Task<CategoryModel?> InsertQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? parameters )
    {
        return await connection.QuerySingleOrDefaultAsync<CategoryModel>( PROCEDURE_INSERT, parameters, transaction, commandType: CommandType.StoredProcedure );
    }
    static async Task<bool> UpdateQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? parameters )
    {
        int rowsAffected = await connection.ExecuteAsync( PROCEDURE_UPDATE, parameters, transaction, commandType: CommandType.StoredProcedure );
        return rowsAffected > 0;
    }
    static async Task<bool> DeleteQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? parameters )
    {
        int rowsAffected = await connection.ExecuteAsync( PROCEDURE_DELETE, parameters, transaction, commandType: CommandType.StoredProcedure );
        return rowsAffected > 0;
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