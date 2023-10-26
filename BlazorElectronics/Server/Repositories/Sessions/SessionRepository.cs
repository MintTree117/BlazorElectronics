using System.Data;
using System.Data.Common;
using BlazorElectronics.Server.DbContext;
using BlazorElectronics.Server.Models.Users;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BlazorElectronics.Server.Repositories.Sessions;

public class SessionRepository : DapperRepository, ISessionRepository
{
    const string STORED_PROCEDURE_CREATE_SESSION = "Create_Session";
    const string STORED_PROCEDURE_GET_SESSION = "Get_Session";

    const string QUERY_PARAM_USER_ID = "@Id";
    const string QUERY_PARAM_SESSION_DATE = "@DateCreated";
    const string QUERY_PARAM_DATE_ACTIVE = "@DateActive";
    const string QUERY_PARAM_IS_ACTIVE = "@IsActive";
    const string QUERY_PARAM_IP_ADDRESS = "@IpAddress";
    const string QUERY_PARAM_SESSION_SALT = "@Salt";
    const string QUERY_PARAM_SESSION_HASH = "@Hash";
    
    public SessionRepository( DapperContext dapperContext ) : base( dapperContext ) { }
    
    public async Task<UserSession?> CreateSession( UserSession session )
    {
        SqlConnection? connection = null;
        DbTransaction? transaction = null;
        
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_ID, session.UserId );
        dynamicParams.Add( QUERY_PARAM_SESSION_DATE, session.DateCreated );
        dynamicParams.Add( QUERY_PARAM_DATE_ACTIVE, session.DateCreated );
        dynamicParams.Add( QUERY_PARAM_IS_ACTIVE, session.IsActive );
        dynamicParams.Add( QUERY_PARAM_IP_ADDRESS, session.IpAddress );
        dynamicParams.Add( QUERY_PARAM_SESSION_SALT, session.Salt );
        dynamicParams.Add( QUERY_PARAM_SESSION_HASH, session.Hash );

        try
        {
            connection = await _dbContext.GetOpenConnection();
            transaction = await connection!.BeginTransactionAsync();
        }
        catch ( SqlException e )
        {
            await HandleConnectionTransactionDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
        catch ( Exception e )
        {
            await HandleConnectionTransactionDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }

        try
        {
            UserSession? insertedSession = 
                await connection.QuerySingleAsync<UserSession>( STORED_PROCEDURE_CREATE_SESSION, dynamicParams, commandType: CommandType.StoredProcedure, transaction: transaction ).ConfigureAwait( false );
            await transaction.CommitAsync();
            await transaction.DisposeAsync();
            await connection.CloseAsync();
            return insertedSession;
        }
        catch ( SqlException e )
        {
            await HandleConnectionTransactionRollbackDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
        catch ( Exception e )
        {
            await HandleConnectionTransactionRollbackDisposal( connection, transaction );
            throw GetRepositoryException( e );
        }
    }
    public async Task<UserSession?> GetSession( int userId, string ipAddress )
    {
        var dynamicParams = new DynamicParameters();
        dynamicParams.Add( QUERY_PARAM_USER_ID, userId );
        dynamicParams.Add( QUERY_PARAM_IP_ADDRESS, ipAddress );

        try
        {
            await using SqlConnection? connection = await _dbContext.GetOpenConnection();
            return await connection.QuerySingleAsync<UserSession>( STORED_PROCEDURE_GET_SESSION, dynamicParams, commandType: CommandType.StoredProcedure );
        }
        catch ( SqlException e )
        {
            throw new RepositoryException( e.Message, e );
        }
        catch ( Exception e )
        {
            throw new RepositoryException( e.Message, e );
        }
    }
    public async Task<bool> UpdateSession( UserSession update )
    {
        throw new NotImplementedException();
    }
}