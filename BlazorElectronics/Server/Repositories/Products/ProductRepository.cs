using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Products;
using Dapper;

namespace BlazorElectronics.Server.Repositories.Products;

public class ProductRepository : DapperRepository, IProductRepository
{
    const string PROCEDURE_GET = "Get_Product";
    const string PROCEDURE_INSERT = "Insert_Product";
    const string PROCEDURE_UPDATE = "Update_Product";
    const string PROCEDURE_DELETE = "Delete_Product";

    public ProductRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
    
    public async Task<ProductModel?> Get( int productId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_PRODUCT_ID, productId );
        return await TryQueryAsync( QuerySingleOrDefault<ProductModel?>, p, PROCEDURE_GET );
    }
    public async Task<int> Insert( ProductModel model )
    {
        DynamicParameters p = GetInsertParams( model );
        return await TryQueryTransactionAsync( QuerySingleOrDefaultTransaction<int>, p, PROCEDURE_INSERT );
    }
    public async Task<bool> Update( ProductModel model )
    {
        DynamicParameters p = GetUpdateParams( model );
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_UPDATE );
    }
    public async Task<bool> Delete( int productId )
    {
        DynamicParameters p = new();
        p.Add( PARAM_PRODUCT_ID, productId );
        return await TryQueryTransactionAsync( Execute, p, PROCEDURE_DELETE );
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

    static DynamicParameters GetInsertParams( ProductModel m )
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
    static DynamicParameters GetUpdateParams( ProductModel m )
    {
        DynamicParameters p = GetInsertParams( m );
        p.Add( PARAM_PRODUCT_ID, m.ProductId );
        return p;
    }
}