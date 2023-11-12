using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Products;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Products;

public class ProductRepository : DapperRepository, IProductRepository
{
    public ProductRepository( DapperContext dapperContext ) : base( dapperContext ) { }

    public async Task<IEnumerable<Product>?> GetAllProducts()
    {
        var productDictionary = new Dictionary<int, Product>();

        try
        {
            await using SqlConnection? connection = await _dbContext.GetOpenConnection();
            return await connection.QueryAsync<Product, ProductVariant, Product>
            ( "STORED_PROCEDURE_GET_ALL_PRODUCTS", ( product, variant ) =>
                {
                    if ( !productDictionary.TryGetValue( product.ProductId, out Product? productEntry ) )
                    {
                        productEntry = product;
                        productDictionary.Add( productEntry.ProductId, productEntry );
                    }
                    if ( variant != null && !product.ProductVariants.Contains( variant ) )
                        product.ProductVariants.Add( variant );
                    return product;
                },
                splitOn: COL_PRODUCT_ID,
                commandType: CommandType.StoredProcedure );
        }
        catch ( SqlException e )
        {
            throw new ServiceException( e.Message, e );
        }
        catch ( Exception e )
        {
            throw new ServiceException( e.Message, e );
        }
    }
}