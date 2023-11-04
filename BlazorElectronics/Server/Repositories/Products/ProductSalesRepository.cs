using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Features;
using BlazorElectronics.Server.Models.Products;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Products;

public class ProductSalesRepository : DapperRepository, IProductSalesRepository
{
    const string STORED_PROCEDURE_GET_FEATURED_SALES = "Get_ProductSalesFeatured";
    const string STORED_PROCEDURE_GET_SALES = "Get_ProductSales";
    
    public ProductSalesRepository( DapperContext dapperContext )
        : base( dapperContext ) { }
    
    public async Task<IEnumerable<ProductSale>?> GetProductSalesFeatured()
    {
        return await TryQueryAsync( GetProductSalesFeaturedQuery, null );
    }
    public async Task<IEnumerable<ProductSale>?> GetProductSales()
    {
        return await TryQueryAsync( GetProductSalesQuery, null );
    }

    static async Task<IEnumerable<ProductSale>?> GetProductSalesFeaturedQuery( SqlConnection connection, DynamicParameters? dynamicParams )
    {
        return await connection.QueryAsync<ProductSale>( STORED_PROCEDURE_GET_FEATURED_SALES, commandType: CommandType.StoredProcedure );
    }
    static async Task<IEnumerable<ProductSale>?> GetProductSalesQuery( SqlConnection connection, DynamicParameters? dynamicParams )
    {
        return await connection.QueryAsync<ProductSale>( STORED_PROCEDURE_GET_SALES, commandType: CommandType.StoredProcedure );
    }
}