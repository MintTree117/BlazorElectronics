using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Shared.Admin.Categories;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Admin.Repositories;

public class AdminCategoryRepository : _AdminRepository, IAdminCategoryRepository
{
    const string PROCEDURE_GET_VIEW = "Get_CategoriesView";
    static readonly string[] PROCEDURES_GET_CATEGORY = { "Get_CategoryPrimaryEdit", "Get_CategorySecondaryEdit", "Get_CategoryTertiaryEdit" };
    static readonly string[] PROCEDURES_INSERT_CATEGORY = { "Insert_CategoryPrimary", "Insert_CategorySecondary", "Insert_CategoryTertiary" };
    static readonly string[] PROCEDURES_UPDATE_CATEGORY = { "Update_CategoryPrimary", "Update_CategorySecondary", "Update_CategoryTertiary" };
    static readonly string[] PROCEDURES_DELETE_CATEGORY = { "Delete_CategoryPrimary", "Delete_CategorySecondary", "Delete_CategoryTertiary" };
    
    public AdminCategoryRepository( DapperContext dapperContext )
        : base( dapperContext ) { }

    public Task<CategoryViewDto?> GetCategoriesView()
    {
        return TryQueryAsync( GetCategoriesViewQuery );
    }
    public async Task<EditCategoryDto?> GetEditCategory( GetCategoryEditDto request )
    {
        string procedure = GetProcedure( PROCEDURES_GET_CATEGORY, request.CategoryTier );
        DynamicParameters parameters = GetEditParameters( request );

        return await TryAdminQuerySingle<EditCategoryDto>( procedure, parameters );
    }
    public async Task<EditCategoryDto?> InsertCategory( AddCategoryDto dto )
    {
        string procedure = GetProcedure( PROCEDURES_INSERT_CATEGORY, dto.Tier );
        DynamicParameters parameters = GetAddParameters( dto );

        return await TryAdminQueryTransaction<EditCategoryDto>( procedure, parameters );
    }
    public async Task<bool> UpdateCategory( EditCategoryDto dto )
    {
        string procedure = GetProcedure( PROCEDURES_UPDATE_CATEGORY, dto.Tier );
        DynamicParameters parameters = GetUpdateParameters( dto );

        return await TryAdminTransaction( procedure, parameters );
    }
    public async Task<bool> DeleteCategory( RemoveCategoryDto dto )
    {
        string procedure = GetProcedure( PROCEDURES_DELETE_CATEGORY, dto.CategoryTier );
        DynamicParameters parameters = GetDeleteParameters( dto );
        
        return await TryAdminTransaction( procedure, parameters );
    }

    static async Task<CategoryViewDto?> GetCategoriesViewQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? parameters )
    {
        SqlMapper.GridReader? multi = await connection.QueryMultipleAsync( PROCEDURE_GET_VIEW, commandType: CommandType.StoredProcedure );

        if ( multi is null )
            return null;

        IEnumerable<CategoryPrimaryViewDto>? primary = await multi.ReadAsync<CategoryPrimaryViewDto>();
        IEnumerable<CategorySecondaryViewDto>? secondary = await multi.ReadAsync<CategorySecondaryViewDto>();
        IEnumerable<CategoryTertiaryViewDto>? tertiary = await multi.ReadAsync<CategoryTertiaryViewDto>();

        return new CategoryViewDto
        {
            Primary = primary is not null ? primary.ToList() : new List<CategoryPrimaryViewDto>(),
            Secondary = secondary is not null ? secondary.ToList() : new List<CategorySecondaryViewDto>(),
            Tertiary = tertiary is not null ? tertiary.ToList() : new List<CategoryTertiaryViewDto>()
        };
    }

    static string GetProcedure( string[] procedures, int tier )
    {
        return procedures[ tier - 1 ];
    }
    static DynamicParameters GetEditParameters( GetCategoryEditDto request )
    {
        var parameters = new DynamicParameters();

        string categoryIdName = request.CategoryTier switch
        {
            1 => PARAM_CATEGORY_PRIMARY_ID,
            2 => PARAM_CATEGORY_SECONDARY_ID,
            3 => PARAM_CATEGORY_TERTIARY_ID,
            _ => throw new ServiceException( "Invalid category tier!", null )
        };

        parameters.Add( categoryIdName, request.CategoryId );
        return parameters;
    }
    static DynamicParameters GetAddParameters( AddCategoryDto dto )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_CATEGORY_NAME, dto.Name );
        parameters.Add( PARAM_CATEGORY_API_URL, dto.ApiUrl );
        parameters.Add( PARAM_CATEGORY_IMAGE_URL, dto.ImageUrl );
        parameters.Add( PARAM_CATEGORY_DESCRIPTION, dto.Description );

        if ( dto.Tier > 1 )
            parameters.Add( PARAM_CATEGORY_PRIMARY_ID, dto.PrimaryCategoryId );

        if ( dto.Tier > 2 )
            parameters.Add( PARAM_CATEGORY_SECONDARY_ID, dto.SecondaryCategoryId );

        return parameters;
    }
    static DynamicParameters GetUpdateParameters( EditCategoryDto dto )
    {
        var parameters = new DynamicParameters();
        parameters.Add( PARAM_CATEGORY_PRIMARY_ID, dto.PrimaryCategoryId );
        parameters.Add( PARAM_CATEGORY_NAME, dto.Name );
        parameters.Add( PARAM_CATEGORY_API_URL, dto.ApiUrl );
        parameters.Add( PARAM_CATEGORY_IMAGE_URL, dto.ImageUrl );
        parameters.Add( PARAM_CATEGORY_DESCRIPTION, dto.Description );

        if ( dto.Tier > 1 )
            parameters.Add( PARAM_CATEGORY_SECONDARY_ID, dto.SecondaryCategoryId );

        if ( dto.Tier > 2 )
            parameters.Add( PARAM_CATEGORY_TERTIARY_ID, dto.TertiaryCategoryId );

        return parameters;
    }
    static DynamicParameters GetDeleteParameters( RemoveCategoryDto dto )
    {
        var parameters = new DynamicParameters();
        
        string paramName = dto.CategoryTier switch
        {
            1 => PARAM_CATEGORY_PRIMARY_ID,
            2 => PARAM_CATEGORY_SECONDARY_ID,
            3 => PARAM_CATEGORY_TERTIARY_ID,
            _ => throw new ServiceException( "Invalid category tier!", null )
        };
        
        parameters.Add( paramName, dto.CategoryId );
        return parameters;
    }
}