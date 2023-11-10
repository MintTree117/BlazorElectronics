using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Categories;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Categories;

public class CategoryRepository : DapperRepository, ICategoryRepository
{
    const string STORED_PROCEDURE_GET_CATEGORIES = "Get_Categories";
    const string STORED_PROCEDURE_GET_DESCRIPTIONS_PRIMARY = "Get_Categories";
    const string STORED_PROCEDURE_GET_DESCRIPTION = "Get_CategoryDescriptionsPrimary";

    const string QUERY_PARAM_CATEGORY_ID = "@CategoryId";
    const string QUERY_PARAM_CATEGORY_TIER = "@CategoryTier";

    public CategoryRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
    
    public async Task<CategoriesModel?> GetCategories()
    {
        SqlMapper.GridReader? multi = await TryQueryAsync( GetCategoriesQuery );

        if ( multi is null )
            return null;
        
        return new CategoriesModel
        {
            Primary = await multi.ReadAsync<PrimaryCategory>(),
            Secondary = await multi.ReadAsync<SecondaryCategory>(),
            Tertiary = await multi.ReadAsync<TertiaryCategory>()
        };
    }
    public async Task<IEnumerable<string>?> GetPrimaryCategoryDescriptions()
    {
        return await TryQueryAsync( GetPrimaryCategoryDescriptionsQuery );
    }
    public async Task<string?> GetCategoryDescription( int categoryId, int categoryTier )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_CATEGORY_ID, categoryId );
        dynamicParams.Add( QUERY_PARAM_CATEGORY_TIER, categoryTier );

        return await TryQueryAsync( GetDescriptionQuery, dynamicParams );
    }

    static async Task<SqlMapper.GridReader?> GetCategoriesQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QueryMultipleAsync( STORED_PROCEDURE_GET_CATEGORIES, commandType: CommandType.StoredProcedure );
    }
    static async Task<IEnumerable<string>?> GetPrimaryCategoryDescriptionsQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QueryAsync<string>( STORED_PROCEDURE_GET_DESCRIPTIONS_PRIMARY, commandType: CommandType.StoredProcedure );
    }
    static async Task<string?> GetDescriptionQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleAsync<string>( STORED_PROCEDURE_GET_DESCRIPTION, commandType: CommandType.StoredProcedure );
    }
}