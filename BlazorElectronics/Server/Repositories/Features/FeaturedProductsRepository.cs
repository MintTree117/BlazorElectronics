using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Features;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Features;

public class FeaturedProductsRepository : DapperRepository<FeaturedProduct>, IFeaturedProductsRepository
{
    const string STORED_PROCEDURE_GET_FEATURED_PRODUCT_BY_ID = "Get_FeaturedProductById";
    const string STORED_PROCEDURE_GET_FEATURED_PRODUCTS = "Get_FeaturedProducts";
    
    public FeaturedProductsRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    public override async Task<IEnumerable<FeaturedProduct>?> GetAll()
    {
        await using SqlConnection connection = await _dbContext.GetOpenConnection();
        return await connection.QueryAsync<FeaturedProduct>( STORED_PROCEDURE_GET_FEATURED_PRODUCTS, commandType: CommandType.StoredProcedure );
    }
    public override async Task<FeaturedProduct?> GetById( int id )
    {
        await using SqlConnection connection = await _dbContext.GetOpenConnection();
        return await connection.QuerySingleAsync<FeaturedProduct>( STORED_PROCEDURE_GET_FEATURED_PRODUCT_BY_ID, id, commandType: CommandType.StoredProcedure );
    }
    public override Task Insert( FeaturedProduct item )
    {
        throw new NotImplementedException();
    }
}