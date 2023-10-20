using System.Data;
using System.Text;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Products;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Products;

public class ProductCartRepository : DapperRepository<Product>, IProductCartRepository
{
    const string QUERY_PARAM_PRODUCT_ID_LIST = "@ProductIdList";
    const string QUERY_PARAM_VARIANT_ID_LIST = "@VariantIdList";
    const string STORED_PROCEDURE_GET_CART_PRODUCTS_BY_ID = "Get_CartProducts";

    public ProductCartRepository( DapperContext dapperContext ) : base( dapperContext ) { }

    public async Task<IEnumerable<Product>?> GetCartItems( List<int> productIds, List<int> variantIds )
    {
        var productIdBuilder = new StringBuilder();
        var variantIdBuilder = new StringBuilder();

        await Task.Run( () =>
        {
            for ( int i = 0; i < productIds.Count; i++ )
            {
                productIdBuilder.Append( $"{productIds[ i ]}" );
                variantIdBuilder.Append( $"{variantIds[ i ]}" );

                if ( i >= productIds.Count - 1 ) 
                    continue;
                
                productIdBuilder.Append( "," );
                variantIdBuilder.Append( "," );
            }
        } );
        
        await using SqlConnection connection = await _dbContext.GetOpenConnection();

        var productDictionary = new Dictionary<int, Product>();
        var dynamicParameters = new DynamicParameters();
        dynamicParameters.Add( QUERY_PARAM_PRODUCT_ID_LIST, productIdBuilder.ToString() );
        dynamicParameters.Add( QUERY_PARAM_VARIANT_ID_LIST, variantIdBuilder.ToString() );

        await connection.QueryAsync<Product, ProductVariant, Product>
        ( STORED_PROCEDURE_GET_CART_PRODUCTS_BY_ID, ( product, variant ) =>
            {
                if ( !productDictionary.TryGetValue( product.ProductId, out Product? productEntry ) )
                {
                    productEntry = product;
                    productDictionary.Add( productEntry.ProductId, productEntry );
                }
                if ( variant != null && productEntry.ProductVariants.Count <= 0 )
                    productEntry.ProductVariants.Add( variant );
                return productEntry;
            },
            dynamicParameters,
            splitOn: SqlConsts.COLUMN_PRODUCT_ID,
            commandType: CommandType.StoredProcedure );

        return productDictionary.Values;
    }
    
    public override Task<IEnumerable<Product>?> GetAll()
    {
        throw new NotImplementedException();
    }
    public override Task<Product?> GetById( int id )
    {
        throw new NotImplementedException();
    }
}