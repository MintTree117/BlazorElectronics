using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Products;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Products;

public class ProductSalesRepository : DapperRepository, IProductSalesRepository
{
    const string PROCEDURE_GET_FEATURED_SALES = "Get_ProductSalesFeatured";
    const string PROCEDURE_GET_SALES = "Get_ProductSales";
    
    public ProductSalesRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    public async Task<IEnumerable<ProductSale>?> GetProductSalesFeatured()
    {
        return await TryQueryAsync( GetProductSalesFeaturedQuery );
    }
    public async Task<IEnumerable<ProductSale>?> GetProductSales()
    {
        return await TryQueryAsync( GetProductSalesQuery );
    }

    static async Task<IEnumerable<ProductSale>?> GetProductSalesFeaturedQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QueryAsync<ProductSale>( PROCEDURE_GET_FEATURED_SALES, commandType: CommandType.StoredProcedure );
    }
    static async Task<IEnumerable<ProductSale>?> GetProductSalesQuery( SqlConnection connection, string? dynamicSql, DynamicParameters? dynamicParams )
    {
        return await connection.QueryAsync<ProductSale>( PROCEDURE_GET_SALES, commandType: CommandType.StoredProcedure );
    }
}