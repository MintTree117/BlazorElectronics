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

    public async Task<CategoryCollections> GetCategories()
    {
        await using SqlConnection connection = _dbContext.CreateConnection();
        await connection.OpenAsync();

        var collections = new CategoryCollections();
        var categoryDictionary = new Dictionary<int, Category>();
        var subCategoryDictionary = new Dictionary<int, CategorySub>();

        await connection.QueryAsync
            <Category, CategorySub, CategoryCollections>
                ( STORED_PROCEDURE_GET_CATEGORIES,
                ( category, categorySub ) => {
                    if ( category != null )
                        categoryDictionary.TryAdd( category.CategoryId, category );
                        //collections.Categories.Add( category );
                    if ( categorySub != null )
                        subCategoryDictionary.TryAdd( categorySub.CategorySubId, categorySub );
                        //collections.CategoriesSub.Add( categorySub );
                    return collections;
                },
                splitOn: CATEGORY_ID_COLUMN,
                commandType: CommandType.StoredProcedure );

        collections.Categories.AddRange( categoryDictionary.Values );
        collections.CategoriesSub.AddRange( subCategoryDictionary.Values );
        
        return collections;
    }
}