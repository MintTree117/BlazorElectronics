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
        return await TryQueryAsync( GetCategoriesQuery, null );
    }
    public async Task<IEnumerable<string?>?> GetPrimaryCategoryDescriptions()
    {
        return await TryQueryAsync( GetPrimaryCategoryDescriptionsQuery, null );
    }
    public async Task<string?> GetCategoryDescription( int categoryId, int categoryTier )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_CATEGORY_ID, categoryId );
        dynamicParams.Add( QUERY_PARAM_CATEGORY_TIER, categoryTier );

        return await TryQueryAsync( GetDescriptionQuery, dynamicParams );
    }

    static async Task<CategoriesModel?> GetCategoriesQuery( SqlConnection connection, DynamicParameters? dynamicParams )
    {
        const string splitOnColumns = $"{COL_CATEGORY_PRIMARY_ID},{COL_CATEGORY_SECONDARY_ID},{COL_CATEGORY_TERTIARY_ID}";

        var models = new CategoriesModel();
        
        Dictionary<int, PrimaryCategory> primaryDict = models.Primary;
        Dictionary<int, SecondaryCategory> secondaryDict = models.Secondary;
        Dictionary<int, TertiaryCategory> tertiaryDict = models.Tertiary;
        
        await connection.QueryAsync<PrimaryCategory, SecondaryCategory, TertiaryCategory, object?>
        ( STORED_PROCEDURE_GET_CATEGORIES,
            ( primary, secondary, tertiary ) =>
            {
                if ( !primaryDict.TryGetValue( primary.PrimaryCategoryId, out PrimaryCategory? primaryEntry ) )
                {
                    primaryEntry = primary;
                    primaryDict.Add( primaryEntry.PrimaryCategoryId, primaryEntry );
                }
                if ( secondary != null )
                {
                    if ( !secondaryDict.TryGetValue( secondary.SecondaryCategoryId, out SecondaryCategory? secondaryEntry ) )
                    {
                        secondaryEntry = secondary;
                        secondaryDict.Add( secondaryEntry.SecondaryCategoryId, secondaryEntry );
                    }
                }
                if ( tertiary != null )
                {
                    if ( !tertiaryDict.TryGetValue( tertiary.TertiaryCategoryId, out TertiaryCategory? tertieryEntry ) )
                    {
                        tertieryEntry = tertiary;
                        tertiaryDict.Add( tertieryEntry.TertiaryCategoryId, tertieryEntry );
                    }
                }
                return null;
            },
            splitOn: splitOnColumns,
            commandType: CommandType.StoredProcedure );

        return models;
    }
    static async Task<IEnumerable<string?>?> GetPrimaryCategoryDescriptionsQuery( SqlConnection connection, DynamicParameters? dynamicParams )
    {
        return await connection.QueryAsync<string?>( STORED_PROCEDURE_GET_DESCRIPTIONS_PRIMARY, commandType: CommandType.StoredProcedure );
    }
    static async Task<string?> GetDescriptionQuery( SqlConnection connection, DynamicParameters? dynamicParams )
    {
        return await connection.QuerySingleAsync<string?>( STORED_PROCEDURE_GET_DESCRIPTION, commandType: CommandType.StoredProcedure );
    }
}