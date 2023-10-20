using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Products;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Products;

public class ProductDealsRepository : DapperRepository<Product>, IProductDealsRepository
{
    const string STORED_PROCEDURE_GET_TOP_DEALS = "Get_TopDeals";
    const string STORED_PROCEDURE_GET_DEAL_BY_ID = "Get_DealById";
    
    public ProductDealsRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    public override async Task<IEnumerable<Product>?> GetAll()
    {
        await using SqlConnection connection = await _dbContext.GetOpenConnection();
        return await connection.QueryAsync<Product>( STORED_PROCEDURE_GET_TOP_DEALS, commandType: CommandType.StoredProcedure );
    }
    public override async Task<Product?> GetById( int id )
    {
        await using SqlConnection connection = await _dbContext.GetOpenConnection();
        return await connection.QueryFirstAsync<Product>( STORED_PROCEDURE_GET_DEAL_BY_ID, commandType: CommandType.StoredProcedure );
    }
}