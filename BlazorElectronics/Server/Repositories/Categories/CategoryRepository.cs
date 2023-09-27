using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Categories;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Categories;

public class CategoryRepository : ICategoryRepository
{
    const string CATEGORY_ID_COLUMN = "CategoryId";
    const string CATEGORY_SUB_ID_COLUMN = "CategorySubId";
    const string STORED_PROCEDURE_GET_CATEGORIES = "Get_Categories";

    readonly DapperContext _dbContext;

    public CategoryRepository( DapperContext dapperContext )
    {
        _dbContext = dapperContext;
    }
    
    public async Task<IEnumerable<Category>?> GetCategories()
    {
        await using SqlConnection connection = _dbContext.CreateConnection();
        await connection.OpenAsync();
        
        var categoryDictionary = new Dictionary<int, Category>();

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
            splitOn: $"{CATEGORY_ID_COLUMN},{CATEGORY_SUB_ID_COLUMN}",
            commandType: CommandType.StoredProcedure );

        return categories;
    }
}