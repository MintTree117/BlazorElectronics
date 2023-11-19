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
    
    protected async Task<T?> TryAdminQuerySingle<T>( string procedure, DynamicParameters paramters )
    {
        SqlConnection connection = await _dbContext.GetOpenConnection();

        try
        {
            var result = await connection.QuerySingleOrDefaultAsync<T>( procedure, paramters, commandType: CommandType.StoredProcedure );
            await connection.CloseAsync();
            return result;
        }
        catch ( SqlException sqlEx )
        {
            await connection.CloseAsync();
            throw new ServiceException( sqlEx.Message, sqlEx );
        }
        catch ( TimeoutException timeoutEx )
        {
            await connection.CloseAsync();
            throw new ServiceException( timeoutEx.Message, timeoutEx );
        }
        catch ( Exception ex )
        {
            await connection.CloseAsync();
            throw new ServiceException( ex.Message, ex );
        }
    }
    protected async Task<T?> TryAdminQueryTransaction<T>( string procedure, DynamicParameters paramters )
    {
        SqlConnection connection = await _dbContext.GetOpenConnection();
        DbTransaction transaction = await connection.BeginTransactionAsync();

        try
        {
            var result = await connection.QuerySingleOrDefaultAsync<T>( procedure, paramters, transaction, commandType: CommandType.StoredProcedure );
            await transaction.CommitAsync();
            await transaction.DisposeAsync();
            await connection.CloseAsync();
            return result;
        }
        catch ( SqlException sqlEx )
        {
            await HandleConnectionTransactionRollbackDisposal( connection, transaction );
            throw new ServiceException( sqlEx.Message, sqlEx );
        }
        catch ( TimeoutException timeoutEx )
        {
            await HandleConnectionTransactionRollbackDisposal( connection, transaction );
            throw new ServiceException( timeoutEx.Message, timeoutEx );
        }
        catch ( Exception ex )
        {
            await HandleConnectionTransactionRollbackDisposal( connection, transaction );
            throw new ServiceException( ex.Message, ex );
        }
    }
    protected async Task<bool> TryAdminTransaction( string procedure, DynamicParameters paramters )
    {
        SqlConnection connection = await _dbContext.GetOpenConnection();
        DbTransaction transaction = await connection.BeginTransactionAsync();

        try
        {
            int result = await connection.ExecuteAsync( procedure, paramters, transaction, commandType: CommandType.StoredProcedure ).ConfigureAwait( false );
            await transaction.CommitAsync();
            await transaction.DisposeAsync();
            await connection.CloseAsync();
            return result > 0;
        }
        catch ( SqlException sqlEx )
        {
            await HandleConnectionTransactionRollbackDisposal( connection, transaction );
            throw new ServiceException( sqlEx.Message, sqlEx );
        }
        catch ( TimeoutException timeoutEx )
        {
            await HandleConnectionTransactionRollbackDisposal( connection, transaction );
            throw new ServiceException( timeoutEx.Message, timeoutEx );
        }
        catch ( Exception ex )
        {
            await HandleConnectionTransactionRollbackDisposal( connection, transaction );
            throw new ServiceException( ex.Message, ex );
        }
    }
}