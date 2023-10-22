using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Specs;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Specs;

public class SpecLookupRepository : DapperRepository<SpecLookup>, ISpecLookupRepository
{
    const string LOOKUP_ID_COLUMN = "LookupId";
    const string CATEGORY_ID_COLUMN = "CategoryId";
    
    const string STORED_PROCEDURE_GET = "Get_SpecLookups";
    const string STORED_PROCEDURE_GET_BY_CATEGORY = "Get_SpecLookupsByCategory";
    
    public SpecLookupRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    public override async Task<IEnumerable<SpecLookup>?> GetAll() 
    {
        await using SqlConnection connection = await _dbContext.GetOpenConnection();
        return await connection.QueryAsync<SpecLookup>( STORED_PROCEDURE_GET, commandType: CommandType.StoredProcedure );
    }
    public override async Task<SpecLookup?> GetById( int id ) { throw new NotImplementedException(); }
    public override Task Insert( SpecLookup item )
    {
        throw new NotImplementedException();
    }
    public async Task<IEnumerable<SpecLookup>> GetByCategory( int categoryId )
    {
        await using SqlConnection connection = await _dbContext.GetOpenConnection();
        var param = new { CategoryId = categoryId };
        return await connection.QueryAsync<SpecLookup>( STORED_PROCEDURE_GET_BY_CATEGORY, param, commandType: CommandType.StoredProcedure );
    }
}