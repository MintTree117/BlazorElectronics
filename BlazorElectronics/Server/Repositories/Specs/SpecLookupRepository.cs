using System.Data;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Specs;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Specs;

public class SpecLookupRepository : DapperRepository<SpecLookup>, ISpecLookupRepository
{
    const string STORED_PROCEDURE_GET_SPEC_LOOKUPS = "GetSpecLookups";
    
    public SpecLookupRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    public override async Task<IEnumerable<SpecLookup>> GetAll() 
    {
        await using SqlConnection connection = await _dbContext.GetOpenConnection();
        return await connection.QueryAsync<SpecLookup>( STORED_PROCEDURE_GET_SPEC_LOOKUPS, commandType: CommandType.StoredProcedure );
    }
    public override async Task<SpecLookup> GetById( int id ) { throw new NotImplementedException(); }
}