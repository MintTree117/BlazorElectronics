using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Categories;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Categories;

public class CategoryRepository : DapperRepository, ICategoryRepository
{
    const string PROCEDURE_GET_CATEGORIES = "Get_Categories";
    const string PROCEDURE_GET_DESCRIPTIONS_PRIMARY = "Get_CategoryDescriptionsPrimary";
    const string PROCEDURE_GET_DESCRIPTION = "Get_CategoryDescription";

    public CategoryRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    public async Task<CategoriesModel?> GetCategories()
    {
        return await TryQueryAsync( GetCategoriesQuery );
    }
    public async Task<IEnumerable<string>?> GetPrimaryCategoryDescriptions()
    {
        return await TryQueryAsync( GetPrimaryCategoryDescriptionsQuery );
    }
    public async Task<string?> GetCategoryDescription( CategoryType categoryType, int categoryId )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( PARAM_CATEGORY_ID, categoryId );
        dynamicParams.Add( PARAM_CATEGORY_TYPE, ( int ) categoryType );

        return await TryQueryAsync( GetDescriptionQuery, dynamicParams );
    }
    
    static async Task<CategoriesModel?> GetCategoriesQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? multi = await connection.QueryMultipleAsync( PROCEDURE_GET_CATEGORIES, commandType: CommandType.StoredProcedure );

        if ( multi is null )
            return null;

        IEnumerable<PrimaryCategoryModel>? primary = await multi.ReadAsync<PrimaryCategoryModel>();
        IEnumerable<SecondaryCategoryModel>? secondary = await multi.ReadAsync<SecondaryCategoryModel>();
        IEnumerable<TertiaryCategoryModel>? tertiary = await multi.ReadAsync<TertiaryCategoryModel>();

        return new CategoriesModel
        {
            Primary = primary,
            Secondary = secondary,
            Tertiary = tertiary
        };
    }
    static async Task<IEnumerable<string>?> GetPrimaryCategoryDescriptionsQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QueryAsync<string>( PROCEDURE_GET_DESCRIPTIONS_PRIMARY, commandType: CommandType.StoredProcedure );
    }
    static async Task<string?> GetDescriptionQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleAsync<string>( PROCEDURE_GET_DESCRIPTION, commandType: CommandType.StoredProcedure );
    }
}