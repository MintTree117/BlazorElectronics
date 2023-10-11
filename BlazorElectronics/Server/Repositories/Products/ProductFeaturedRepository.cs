using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Products;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Products;

public class ProductFeaturedRepository : DapperRepository<ProductFeatured>, IProductFeaturedRepository
{
    const string STORED_PROCEDURE_GET_FEATURED_PRODUCTS = "Get_FeaturedProducts";
    
    public ProductFeaturedRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    public override async Task<IEnumerable<ProductFeatured>?> GetAll()
    {
        await using SqlConnection connection = await _dbContext.GetOpenConnection();
        return await connection.QueryAsync<ProductFeatured>( STORED_PROCEDURE_GET_FEATURED_PRODUCTS, commandType: CommandType.StoredProcedure );
    }
    public override async Task<ProductFeatured?> GetById( int id )
    {
        throw new NotImplementedException();
    }
}