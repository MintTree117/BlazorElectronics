using System.Data;
using BlazorElectronics.Server.Core.Interfaces;
using BlazorElectronics.Server.Core.Models.Products;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Data.Repositories;

public class ProductRepository : DapperRepository, IProductRepository
{
    const string PROCEDURE_GET = "Get_Product";
    const string PROCEDURE_INSERT = "Insert_Product";
    const string PROCEDURE_UPDATE = "Update_Product";
    const string PROCEDURE_DELETE = "Delete_Product";

    const string PROCEDURE_UPDATE_PRODUCT_RATING = "Update_ProductRatings";
    const string PROCEDURE_UPDATE_PRODUCT_REVIEW_COUNT = "Update_ProductReviewCount";

    public ProductRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
    
    public async Task<ProductModel?> Get( int productId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_PRODUCT_ID, productId );
        return await TryQueryAsync( QuerySingleOrDefault<ProductModel?>, p, PROCEDURE_GET );
    }
    public async Task<int> Insert( ProductEditModel editModel )
    {
        DynamicParameters p = GetInsertParams( editModel );
        return await TryQueryTransactionAsync( QuerySingleOrDefaultTransaction<int>, p, PROCEDURE_INSERT );
    }
    public async Task<bool> Update( ProductEditModel editModel )
    {
        DynamicParameters p = GetUpdateParams( editModel );
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_UPDATE );
    }
    public async Task<bool> Delete( int productId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_PRODUCT_ID, productId );
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_DELETE );
    }
    public async Task<bool> UpdateRatings()
    {
        return await TryQueryTransactionAsync( Execute, null, PROCEDURE_UPDATE_PRODUCT_RATING );
    }
    public async Task<bool> UpdateReviewCount()
    {
        return await TryQueryTransactionAsync( Execute, null, PROCEDURE_UPDATE_PRODUCT_REVIEW_COUNT );
    }

    static async Task<ProductModel?> GetQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? multi = await connection.QueryMultipleAsync( PROCEDURE_GET, dynamicParams, commandType: CommandType.StoredProcedure );

        if ( multi is null )
            return null;

        return new ProductModel
        {
            Product = await multi.ReadSingleOrDefaultAsync<ProductSummaryModel>(),
            Description = await multi.ReadSingleOrDefaultAsync<ProductDescriptionModel>(),
            Categories = await multi.ReadAsync<ProductCategoryModel>(),
            Images = await multi.ReadAsync<ProductImageModel>(),
            SpecLookups = await multi.ReadAsync<ProductSpecLookupModel>(),
            XmlSpecs = await multi.ReadSingleOrDefaultAsync<ProductXmlModel>()
        };
    }

    static DataTable GetCategoriesTable( List<int> categories )
    {
        DataTable table = new();
        table.Columns.Add( COL_CATEGORY_ID, typeof( int ) );

        foreach ( int c in categories )
        {
            DataRow row = table.NewRow();
            row[ COL_CATEGORY_ID ] = c;
            table.Rows.Add( row );
        }

        return table;
    }
    static DataTable GetSpecsTable( Dictionary<int, List<int>> specs )
    {
        DataTable table = new();
        table.Columns.Add( COL_SPEC_ID, typeof( int ) );
        table.Columns.Add( COL_SPEC_VALUE_ID, typeof( int ) );

        foreach ( int s in specs.Keys )
        {
            foreach ( int v in specs[ s ] )
            {
                DataRow row = table.NewRow();
                row[ COL_SPEC_ID ] = s;
                row[ COL_SPEC_VALUE_ID ] = v;
                table.Rows.Add( row );
            }
        }

        return table;
    }
    static DataTable GetImagesTable( List<string> images )
    {
        DataTable table = new();
        table.Columns.Add( COL_PRODUCT_IMAGE, typeof( string ) );

        foreach ( string i in images )
        {
            DataRow row = table.NewRow();
            row[ COL_PRODUCT_IMAGE ] = i;
            table.Rows.Add( row );
        }

        return table;
    }

    static DynamicParameters GetInsertParams( ProductEditModel m )
    {
        DynamicParameters p = new();
        p.Add( PARAM_VENDOR_ID, m.VendorId );
        p.Add( PARAM_PRODUCT_TITLE, m.Title );
        p.Add( PARAM_PRODUCT_THUMBNAIL, m.ThumbnailUrl );
        p.Add( PARAM_PRODUCT_PRICE, m.Price );
        p.Add( PARAM_PRODUCT_SALE_PRICE, m.SalePrice );
        p.Add( PARAM_PRODUCT_RELEASE_DATE, m.ReleaseDate );
        
        p.Add( PARAM_PRODUCT_DESCR, m.Description );

        p.Add( PARAM_PRODUCT_IMAGES, GetImagesTable( m.Images ).AsTableValuedParameter( TVP_PRODUCT_IMAGES ) );
        p.Add( PARAM_CATEGORY_IDS, GetCategoriesTable( m.Categories ).AsTableValuedParameter( TVP_CATEGORY_IDS ) );
        p.Add( PARAM_SPECS, GetSpecsTable( m.SpecLookups ).AsTableValuedParameter( TVP_SPECS ) );

        p.Add( PARAM_PRODUCT_XML, m.XmlSpecs );

        return p;
    }
    static DynamicParameters GetUpdateParams( ProductEditModel m )
    {
        DynamicParameters p = GetInsertParams( m );
        p.Add( PARAM_PRODUCT_ID, m.ProductId );
        return p;
    }
}