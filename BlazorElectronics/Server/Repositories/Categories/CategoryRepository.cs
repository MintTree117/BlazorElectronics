using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Categories;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Categories;

public class CategoryRepository : DapperRepository<Category>, ICategoryRepository
{
    const string STORED_PROCEDURE_GET_CATEGORY_BY_ID = "Get_CategoryById";
    const string STORED_PROCEDURE_GET_CATEGORIES = "Get_Categories";

    public CategoryRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    public override async Task<IEnumerable<Category>?> GetAll()
    {
        await using SqlConnection connection = await _dbContext.GetOpenConnection();
        
        var categoryDictionary = new Dictionary<int, Category>();
        const string splitOnColumns = $"{SqlConsts.COLUMN_CATEGORY_ID},{SqlConsts.COLUMN_CATEGORY_SUB_ID}";

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

        return categories;
    }
    public override async Task<Category?> GetById( int id )
    {
        await using SqlConnection connection = await _dbContext.GetOpenConnection();
        return await connection.QuerySingleAsync<Category?>( STORED_PROCEDURE_GET_CATEGORY_BY_ID, id, commandType: CommandType.StoredProcedure );
    }
}