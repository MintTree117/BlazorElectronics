using System.Data;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Shared.Categories;
using Dapper;

namespace BlazorElectronics.Server.Data.Repositories;

public class CategoryRepository : DapperRepository, ICategoryRepository
{
    const string PROCEDURE_GET = "Get_Categories";
    const string PROCEDURE_GET_EDIT = "Get_Category";
    const string PROCEDURE_BULK_INSERT = "Insert_CategoriesBulk";
    const string PROCEDURE_INSERT = "Insert_Category";
    const string PROCEDURE_UPDATE = "Update_Category";
    const string PROCEDURE_DELETE = "Delete_Category";

    public CategoryRepository( DapperContext dapperContext )
        : base( dapperContext ) { }

    public async Task<IEnumerable<CategoryFullDto>?> Get()
    {
        return await TryQueryAsync( Query<CategoryFullDto>, null, PROCEDURE_GET );
    }
    public async Task<CategoryFullDto?> GetEdit( int categoryId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_CATEGORY_ID, categoryId );
        return await TryQueryAsync( QuerySingleOrDefault<CategoryFullDto?>, p, PROCEDURE_GET_EDIT );
    }
    public async Task<bool> BulkInsert( List<CategoryEditDto> dtos )
    {
        DataTable table = new();
        table.Columns.Add( COL_CATEGORY_PARENT_ID, typeof( int ) );
        table.Columns.Add( COL_CATEGORY_TIER, typeof( int ) );
        table.Columns.Add( COL_CATEGORY_NAME, typeof( string ) );
        table.Columns.Add( COL_CATEGORY_URL, typeof( string ) );
        table.Columns.Add( COL_CATEGORY_IMAGE, typeof( string ) );

        foreach ( CategoryEditDto c in dtos )
        {
            DataRow row = table.NewRow();
            row[ COL_CATEGORY_PARENT_ID ] = c.ParentCategoryId;
            row[ COL_CATEGORY_TIER ] = c.Tier;
            row[ COL_CATEGORY_NAME ] = c.Name;
            row[ COL_CATEGORY_URL ] = c.ApiUrl;
            row[ COL_CATEGORY_IMAGE ] = c.ImageUrl;

            table.Rows.Add( row );
        }

        DynamicParameters p = new();
        p.Add( PARAM_CATEGORIES, table.AsTableValuedParameter( TVP_CATEGORIES ) );

        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_BULK_INSERT );
    }
    public async Task<int> Insert( CategoryEditDto dto )
    {
        DynamicParameters p = GetInsertParameters( dto );
        return await TryQueryTransactionAsync( QuerySingleOrDefaultTransaction<int>, p, PROCEDURE_INSERT );
    }
    public async Task<bool> Update( CategoryEditDto dto )
    {
        DynamicParameters p = GetUpdateParameters( dto );
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_UPDATE );
    }
    public async Task<bool> Delete( int categoryId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_CATEGORY_ID, categoryId );
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_DELETE );
    }
    
    static DynamicParameters GetInsertParameters( CategoryEditDto dto )
    {
        DynamicParameters p = new();
        p.Add( PARAM_CATEGORY_PARENT_ID, dto.ParentCategoryId );
        p.Add( PARAM_CATEGORY_TIER, dto.Tier );
        p.Add( PARAM_CATEGORY_NAME, dto.Name );
        p.Add( PARAM_CATEGORY_API_URL, dto.ApiUrl );
        p.Add( PARAM_CATEGORY_IMAGE_URL, dto.ImageUrl );

        return p;
    }
    static DynamicParameters GetUpdateParameters( CategoryEditDto dto )
    {
        DynamicParameters p = GetInsertParameters( dto );
        p.Add( PARAM_CATEGORY_ID, dto.CategoryId );
        return p;
    }
}