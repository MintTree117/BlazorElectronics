using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using Dapper;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Dtos.Specs;
using BlazorElectronics.Server.Models.Products;
using BlazorElectronics.Server.Models.Products.Specs;

namespace BlazorElectronics.Server.Repositories.Products;

public class ProductDetailsRepository : DapperRepository, IProductDetailsRepository
{
    const string PROCEDURE_GET_PRODUCT_OVERVIEW = "Get_ProductOverview";

    public ProductDetailsRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
    
    public async Task<ProductOverviewModel?> GetProductOverview( int productId )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( PARAM_PRODUCT_ID, productId );

        return await TryQueryAsync( GetProductOverviewQuery, dynamicParams );
    }
    public async Task<ProductSpecsModel?> GetProductSpecs( int productId, int primaryCategoryId, CachedSpecData specData )
    {
        throw new NotImplementedException();
    }


    static async Task<ProductOverviewModel?> GetProductOverviewQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        SqlMapper.GridReader? result = await connection.QueryMultipleAsync( PROCEDURE_GET_PRODUCT_OVERVIEW, dynamicParams, commandType: CommandType.StoredProcedure ).ConfigureAwait( false );

        var productDetails = new ProductOverviewModel();
        
        productDetails.Product = result.Read<Product>().FirstOrDefault();
        productDetails.Product.ProductVariants = result.Read<ProductVariant>().ToList();
        productDetails.ProductDescription = result.Read<ProductDescription>().FirstOrDefault();
        productDetails.ProductImages = result.Read<ProductImage>().ToList();

        return productDetails;
    }

    static async Task<string> BuildProductSpecsQuery( int productId, int primaryCategoryId, CachedSpecData specData )
    {
        return await Task.Run( () =>
        {
            var dynamicParams = new DynamicParameters();

            var builder = new StringBuilder();
            
            builder.Append( $"SELECT * FROM {TABLE_PRODUCT_SPECS_INT}" );
            
            var idTable = new DataTable();
            idTable.Columns.Add( "SpecId", typeof( int ) );
            idTable.Columns.Add( "SpecValueId", typeof( int ) );
            
            
            return builder.ToString();
        } );
    }
}