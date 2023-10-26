using System.Data.Common;
using BlazorElectronics.Server.DbContext;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories;

public abstract class DapperRepository
{
    protected readonly DapperContext _dbContext;

    protected DapperRepository( DapperContext dapperContext ) { _dbContext = dapperContext; }

    protected RepositoryException GetRepositoryException( SqlException e )
    {
        return new RepositoryException( e.Message, e );
    }
    protected RepositoryException GetRepositoryException( Exception e )
    {
        return new RepositoryException( e.Message, e );
    }
    
    protected static async Task HandleConnectionTransactionDisposal( SqlConnection? connection, DbTransaction? transaction = null )
    {
        if ( transaction != null )
            await transaction.DisposeAsync();
        if ( connection != null )
            await connection.CloseAsync();
    }
    protected static async Task HandleConnectionTransactionRollbackDisposal( SqlConnection? connection, DbTransaction? transaction = null )
    {
        if ( transaction != null )
        {
            await transaction.RollbackAsync();
            await transaction.DisposeAsync();
        }
        if ( connection != null )
            await connection.CloseAsync();
    }
}