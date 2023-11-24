using System.Data;
using System.Data.Common;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Shared.Admin.Categories;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Admin.Repositories;

public class AdminCategoryRepository : _AdminRepository, IAdminCategoryRepository
{
    const string PROCEDURE_GET_VIEW = "Get_CategoriesView";
    static readonly string[] PROCEDURES_GET_EDIT = { "Get_CategoryPrimaryEdit", "Get_CategorySecondaryEdit", "Get_CategoryTertiaryEdit" };
    static readonly string[] PROCEDURES_INSERT = { "Insert_CategoryPrimary", "Insert_CategorySecondary", "Insert_CategoryTertiary" };
    static readonly string[] PROCEDURES_UPDATE = { "Update_CategoryPrimary", "Update_CategorySecondary", "Update_CategoryTertiary" };
    static readonly string[] PROCEDURES_DELETE = { "Delete_CategoryPrimary", "Delete_CategorySecondary", "Delete_CategoryTertiary" };
    
    public AdminCategoryRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
    
    public async Task<CategoriesViewDto?> GetView()
    {
        return await TryQueryAsync( GetViewQuery );
    }
    public async Task<CategoryEditDto?> GetEdit( CategoryGetEditDto request )
    {
        string procedure = GetProcedure( PROCEDURES_GET_EDIT, request.CategoryType );
        DynamicParameters parameters = GetEditParameters( request );

        CategoryEditDto? result = await TryQueryAsync( GetEditQuery, parameters, procedure );

        if ( result is null )
            return null;
        
        result.Type = request.CategoryType;
        return result;
    }
    public async Task<CategoryEditDto?> Insert( CategoryAddDto dto )
    {
        string procedure = GetProcedure( PROCEDURES_INSERT, dto.Type );
        DynamicParameters parameters = GetAddParameters( dto );

        CategoryEditDto? result = await TryQueryTransactionAsync( InsertQuery, parameters, procedure );

        if ( result is null )
            return null;

        result.Type = dto.Type;
        return result;
    }
    public async Task<bool> Update( CategoryEditDto dto )
    {
        string procedure = GetProcedure( PROCEDURES_UPDATE, dto.Type );
        DynamicParameters parameters = GetUpdateParameters( dto );

        return await TryQueryTransactionAsync( UpdateQuery, parameters, procedure );
    }
    public async Task<bool> Delete( CategoryRemoveDto dto )
    {
        string procedure = GetProcedure( PROCEDURES_DELETE, dto.CategoryType );
        DynamicParameters parameters = GetDeleteParameters( dto );

        return await TryQueryTransactionAsync( DeleteQuery, parameters, procedure );
    }
    
    static async Task<CategoriesViewDto?> GetViewQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? parameters )
    {
        SqlMapper.GridReader? multi = await connection.QueryMultipleAsync( PROCEDURE_GET_VIEW, commandType: CommandType.StoredProcedure );

        if ( multi is null )
            return null;

        IEnumerable<CategoryPrimaryViewDto>? primary = await multi.ReadAsync<CategoryPrimaryViewDto>();
        IEnumerable<CategorySecondaryViewDto>? secondary = await multi.ReadAsync<CategorySecondaryViewDto>();
        IEnumerable<CategoryTertiaryViewDto>? tertiary = await multi.ReadAsync<CategoryTertiaryViewDto>();

        return new CategoriesViewDto
        {
            Primary = primary is not null ? primary.ToList() : new List<CategoryPrimaryViewDto>(),
            Secondary = secondary is not null ? secondary.ToList() : new List<CategorySecondaryViewDto>(),
            Tertiary = tertiary is not null ? tertiary.ToList() : new List<CategoryTertiaryViewDto>()
        };
    }
    static async Task<CategoryEditDto?> GetEditQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? parameters )
    {
        return await connection.QuerySingleOrDefaultAsync<CategoryEditDto>( dynamicSql, parameters, commandType: CommandType.StoredProcedure );
    }
    static async Task<CategoryEditDto?> InsertQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? parameters )
    {
        return await connection.QuerySingleOrDefaultAsync<CategoryEditDto>( dynamicSql, parameters, transaction, commandType: CommandType.StoredProcedure );
    }
    static async Task<bool> UpdateQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? parameters )
    {
        int rowsAffected = await connection.ExecuteAsync( dynamicSql, parameters, transaction, commandType: CommandType.StoredProcedure );
        return rowsAffected > 0;
    }
    static async Task<bool> DeleteQuery( SqlConnection connection, DbTransaction transaction, string? dynamicSql, DynamicParameters? parameters )
    {
        int rowsAffected = await connection.ExecuteAsync( dynamicSql, parameters, transaction, commandType: CommandType.StoredProcedure );
        return rowsAffected > 0;
    }
    
    static string GetProcedure( string[] procedures, CategoryType type )
    {
        int index = ( int ) type - 1;

        if ( index < 0 || index >= procedures.Length )
            return string.Empty;
        
        return procedures[ index ];
    }
    static DynamicParameters GetEditParameters( CategoryGetEditDto request )
    {
        var parameters = new DynamicParameters();

        string categoryIdName = request.CategoryType switch
        {
            CategoryType.PRIMARY => PARAM_CATEGORY_PRIMARY_ID,
            CategoryType.SECONDARY => PARAM_CATEGORY_SECONDARY_ID,
            CategoryType.TERTIARY => PARAM_CATEGORY_TERTIARY_ID,
            _ => throw new ServiceException( "Invalid category tier!", null )
        };

        parameters.Add( categoryIdName, request.CategoryId );
        return parameters;
    }
    static DynamicParameters GetAddParameters( CategoryAddDto dto )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_CATEGORY_NAME, dto.Name );
        parameters.Add( PARAM_CATEGORY_API_URL, dto.ApiUrl );
        parameters.Add( PARAM_CATEGORY_IMAGE_URL, dto.ImageUrl );
        parameters.Add( PARAM_CATEGORY_DESCRIPTION, dto.Description );

        switch ( dto.Type )
        {
            case CategoryType.SECONDARY:
                parameters.Add( PARAM_CATEGORY_PRIMARY_ID, dto.PrimaryCategoryId );
                break;
            case CategoryType.TERTIARY:
                parameters.Add( PARAM_CATEGORY_PRIMARY_ID, dto.PrimaryCategoryId );
                parameters.Add( PARAM_CATEGORY_SECONDARY_ID, dto.SecondaryCategoryId );
                break;
        }
        
        return parameters;
    }
    static DynamicParameters GetUpdateParameters( CategoryEditDto dto )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_CATEGORY_NAME, dto.Name );
        parameters.Add( PARAM_CATEGORY_API_URL, dto.ApiUrl );
        parameters.Add( PARAM_CATEGORY_IMAGE_URL, dto.ImageUrl );
        parameters.Add( PARAM_CATEGORY_DESCRIPTION, dto.Description );

        switch ( dto.Type )
        {
            case CategoryType.PRIMARY:
                parameters.Add( PARAM_CATEGORY_PRIMARY_ID, dto.PrimaryCategoryId );
                break;
            case CategoryType.SECONDARY:
                parameters.Add( PARAM_CATEGORY_PRIMARY_ID, dto.PrimaryCategoryId );
                parameters.Add( PARAM_CATEGORY_SECONDARY_ID, dto.SecondaryCategoryId );
                break;
            case CategoryType.TERTIARY:
                parameters.Add( PARAM_CATEGORY_PRIMARY_ID, dto.PrimaryCategoryId );
                parameters.Add( PARAM_CATEGORY_SECONDARY_ID, dto.SecondaryCategoryId );
                parameters.Add( PARAM_CATEGORY_TERTIARY_ID, dto.TertiaryCategoryId );
                break;
        }
        
        return parameters;
    }
    static DynamicParameters GetDeleteParameters( CategoryRemoveDto dto )
    {
        var parameters = new DynamicParameters();
        
        string paramName = dto.CategoryType switch
        {
            CategoryType.PRIMARY => PARAM_CATEGORY_PRIMARY_ID,
            CategoryType.SECONDARY => PARAM_CATEGORY_SECONDARY_ID,
            CategoryType.TERTIARY => PARAM_CATEGORY_TERTIARY_ID,
            _ => throw new ServiceException( "Invalid category tier!", null )
        };
        
        parameters.Add( paramName, dto.CategoryId );
        return parameters;
    }
}