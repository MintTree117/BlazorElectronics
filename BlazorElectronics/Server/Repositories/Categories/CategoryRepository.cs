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

        var categoryDictionary = new Dictionary<int, Category>();
        var subCategories = new List<CategorySub>();

        await connection.QueryAsync
            <Category, CategorySub, CategoryCollections>
                ( STORED_PROCEDURE_GET_CATEGORIES,
                ( category, categorySub ) => {
                    if ( !categoryDictionary.TryGetValue( category.CategoryId, out Category? categoryEntry ) ) {
                        categoryEntry = category;
                        categoryDictionary.Add( category.CategoryId, categoryEntry );
                    }
                    if ( categorySub != null )
                        subCategories.Add( categorySub );
                    return new CategoryCollections();
                },
                splitOn: CATEGORY_ID_COLUMN,
                commandType: CommandType.StoredProcedure );

        return new CategoryCollections {
            Categories = categoryDictionary.Values,
            CategoriesSub = subCategories
        };
    }
}