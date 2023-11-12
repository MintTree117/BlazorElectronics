using System.Data;
using System.Data.Common;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Repositories;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Admin.Repositories;

public class _AdminRepository : DapperRepository
{
    protected _AdminRepository( DapperContext dapperContext ) : base( dapperContext ) { }

    protected async Task<bool> ExecuteAdminTransaction( string procedure, DynamicParameters paramters )
    {
        SqlConnection connection = await _dbContext.GetOpenConnection();
        DbTransaction transaction = await connection.BeginTransactionAsync();

        try
        {
            int result = await connection.ExecuteAsync( procedure, paramters, transaction, commandType: CommandType.StoredProcedure );
            await transaction.CommitAsync();
            await transaction.DisposeAsync();
            await connection.CloseAsync();
            return result > 0;
        }
        catch
        {
            await HandleConnectionTransactionRollbackDisposal( connection, transaction );
            throw;
        }
    }
}