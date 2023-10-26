using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Categories;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Categories;

public class CategoryRepository : DapperRepository, ICategoryRepository
{
    const string STORED_PROCEDURE_GET_CATEGORY_BY_ID = "Get_CategoryById";
    const string STORED_PROCEDURE_GET_CATEGORIES = "Get_Categories";

    public CategoryRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    public async Task<IEnumerable<Category>?> GetCategories()
    {
        var categoryDictionary = new Dictionary<int, Category>();
        const string splitOnColumns = $"{SqlConsts.COLUMN_CATEGORY_ID},{SqlConsts.COLUMN_CATEGORY_SUB_ID}";

        try
        {
            await using SqlConnection? connection = await _dbContext.GetOpenConnection();
            IEnumerable<Category>? categories = await connection.QueryAsync<Category, CategorySub, Category>
            ( STORED_PROCEDURE_GET_CATEGORIES,
                ( category, categorySub ) =>
                {
                    if ( !categoryDictionary.TryGetValue( category.CategoryId, out Category? categoryEntry ) )
                    {
                        categoryEntry = category;
                        categoryDictionary.Add( categoryEntry.CategoryId, categoryEntry );
                    }
                    if ( categorySub != null && !categoryEntry.SubCategories.Contains( categorySub ) )
                        category.SubCategories.Add( categorySub );
                    return categoryEntry;
                },
                splitOn: splitOnColumns,
                commandType: CommandType.StoredProcedure );

            return categoryDictionary.Values;
        }
        catch ( SqlException e )
        {
            throw new RepositoryException( e.Message, e );
        }
        catch ( Exception e )
        {
            throw new RepositoryException( e.Message, e );
        }
    }
}