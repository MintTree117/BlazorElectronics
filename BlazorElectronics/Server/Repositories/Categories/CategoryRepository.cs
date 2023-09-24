using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Categories;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Categories;

public class CategoryRepository : ICategoryRepository
{
    const string CATEGORY_ID_COLUMN = "CategoryId";
    const string STORED_PROCEDURE_GET_CATEGORIES = "Get_Categories";

    readonly DapperContext _dbContext;

    public CategoryRepository( DapperContext dapperContext )
    {
        _dbContext = dapperContext;
    }
    
    public async Task<List<Category>> GetCategories()
    {
        await using SqlConnection connection = _dbContext.CreateConnection();
        await connection.OpenAsync();

        var categories = new List<Category>();
        var categoryDictionary = new Dictionary<int, Category>();
        var subCategoryDictionary = new Dictionary<int, CategorySub>();

        await connection.QueryAsync
            <Category, CategorySub, List<Category>>
                ( STORED_PROCEDURE_GET_CATEGORIES,
                ( category, categorySub ) => {
                    if ( category == null ) 
                        return categories;
                    categoryDictionary.TryAdd( category.CategoryId, category );
                    if ( categorySub != null && subCategoryDictionary.TryAdd( categorySub.CategorySubId, categorySub ) )
                        category.SubCategories.Add( categorySub );
                    return categories;
                },
                splitOn: CATEGORY_ID_COLUMN,
                commandType: CommandType.StoredProcedure );

        categories = categoryDictionary.Values.ToList();

        return categories;
    }
}